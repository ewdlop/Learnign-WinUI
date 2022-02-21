using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SilkDotNetLibrary.OpenGL.Buffers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SilkDotNetLibrary.OpenGL.Models;
//https://learnopengl.com/code_viewer_gh.php?code=includes/learnopengl/model.h

public struct Vertex
{
    public Vector3 Position { get; init; }
    public Vector3 Normal { get; init; }
    public Vector2 TexCoords { get; init; }
    public Vector3 Tangent { get; init; }
    public Vector3 BiTangent { get; init; }
    //public int* BonesID = stackalloc int*[5];
    //public Span<int> BonesID = stackalloc int*[5];
}
public unsafe ref struct RefVertex
{
    public Vector3 Position { get; init; }
    public Vector3 Normal { get; init; }
    public Vector2 TexCoords { get; init; }
    public Vector3 Tangent { get; init; }
    public Vector3 BiTangent { get; init; }
    public Span<int> BonesID { get; init; } = stackalloc int[5];
    public Span<float> Weights { get; init; } = stackalloc float[5];
}

public record struct Texture
{
    public uint id { get; }
}

public record struct Mesh
{
    //private Vertex[] Vertices { get; }
    //private uint[] Indices { get; }
    //private Texture[] Textures { get; }
    private uint indicesLength;
    private VertexArrayBufferObject<Vertex, uint> Vao { get; }
    private BufferObject<Vertex> Vbo { get; }
    private BufferObject<uint> Ebo { get; }

    public Mesh(GL gl, ReadOnlySpan<Vertex> vertices, ReadOnlySpan<uint> indices, ReadOnlySpan<Texture> textures)
    {
        indicesLength = Convert.ToUInt32(indices.Length);
        Vbo = new BufferObject<Vertex>(gl, vertices, BufferTargetARB.ArrayBuffer);
        Ebo = new BufferObject<uint>(gl, indices, BufferTargetARB.ElementArrayBuffer);
        Vao = new VertexArrayBufferObject<Vertex, uint>(gl, Vbo, Ebo);
        Vao.VertexAttributePointer(gl, 0, 3, VertexAttribPointerType.Float, 1, 0);
        Vao.VertexAttributePointer(gl, 1, 3, VertexAttribPointerType.Float, 1,
            Marshal.OffsetOf(typeof(Vertex), "Normal").ToInt32());
        Vao.VertexAttributePointer(gl, 2, 2, VertexAttribPointerType.Float, 1,
            Marshal.OffsetOf(typeof(Vertex), "TexCoords").ToInt32());
    }

    public void Draw(GL gl, SilkDotNetLibrary.OpenGL.Shaders.Shader shader, ReadOnlySpan<Texture> textures)
    {
        uint diffuseNr = 1;
        uint specularNr = 1;
        uint normalNr = 1;
        uint heightNr = 1;
        for (int i = 0; i < textures.Length; i++)
        {
            gl.ActiveTexture(GLEnum.Texture0 + i); // active proper texture unit before binding
            // retrieve texture number (the N in diffuse_textureN)
            string name = string.Empty;//textures[i].type;
            string combined = name switch
            {
                "texture_diffuse" => $"{name}{diffuseNr++}",
                "texture_specular" => $"{name}{specularNr++}",
                "texture_normal" => $"{name}{normalNr++}",
                "texture_height" => $"{name}{heightNr++}",
                _ => string.Empty
            };

            // now set the sampler to the correct texture unit
            gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, combined), i);
            // and finally bind the texture
            gl.BindTexture(GLEnum.Texture2D, textures[i].id);
        }
        // draw mesh
        Vao.BindBy(gl);
        gl.DrawElements(GLEnum.Triangles, indicesLength, GLEnum.UnsignedInt, 0);
        gl.BindVertexArray(0);
        // always good practice to set everything back to defaults once configured.
        gl.ActiveTexture(GLEnum.Texture0);
    }
}

public static class X
{
    public static T[] ToAppendedArray<T>(this ReadOnlySpan<T> array, T element) where T : unmanaged
    {
        Span<T> newArray = stackalloc T[array.Length + 1];
        array.CopyTo(newArray);
        newArray[array.Length] = element;
        return newArray.ToArray();
    }
    public static T[] ToAppendedArray<T>(this ReadOnlySpan<T> array, ReadOnlySpan<T> element) where T : unmanaged
    {
        Span<T> newArray = stackalloc T[array.Length + element.Length];
        array.CopyTo(newArray);
        for (int i = 0; i < element.Length; i++)
        {
            newArray[i + array.Length] = element[i];
        }
        return newArray.ToArray();
    }
}

public class Model
{
    public Mesh[] Meshes { get; set; }
    public Texture[] TextureLoaded { get; private set; }
    public string Directory { get; private set; }
    public bool GammaCoorection { get; }
}
public class ModelFactory
{
    private readonly GL _gl;
    private readonly Assimp _assimp;

    public ModelFactory (GL gl)
    {
        _gl = gl;
        _assimp = Assimp.GetApi();
    }

    public unsafe Model LoadModel(string path)
    {
        Scene* scene = _assimp.ImportFile(path, Convert.ToUInt32(PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs));
        if (scene is not null ||
            Convert.ToBoolean(scene->MFlags & Convert.ToUInt32(SceneFlags.Incomplete)) ||
            scene->MRootNode is not null)
        {
            Console.Write($"ERROR::ASSIMP:{_assimp.GetErrorStringS()}");
            return null;
        }
        string Directory = new FileInfo(path).Directory?.Parent?.ToString() ??
                    throw new FileNotFoundException("RIP", path);
        Model model = new()
        {
            Meshes = ProcessNode(scene->MRootNode, scene)
        };
        return model;
    }

    public unsafe Mesh[] ProcessNode(Node* node, Scene* scene)
    {
        IEnumerable<Mesh> meshes = Enumerable.Empty<Mesh>();
        for (uint i = 0; i < node->MNumMeshes; i++)
        {
            Silk.NET.Assimp.Mesh* mesh = scene->MMeshes[node->MMeshes[i]];
            meshes.Append(ProcessMesh(mesh, scene));
        }
        for (int i = 0; i < node->MNumChildren; i++)
        {
            meshes.Concat(ProcessNode(node->MChildren[i], scene));
        }
        return meshes.ToArray();
    }

    public unsafe Mesh ProcessMesh(Silk.NET.Assimp.Mesh* mesh, Scene* scene)
    {
        int verticesSize = (int)mesh->MNumVertices;
        int indicesSize = (int)mesh->MNumFaces;
        Span<Vertex> vertices = stackalloc Vertex[verticesSize];
        Span<uint> indices = stackalloc uint[indicesSize];
        Span<Texture> textures = stackalloc Texture[verticesSize];
        for (int i = 0; i < verticesSize; i++)
        {
            Vertex vertex = new()
            {
                Position = mesh->MVertices[i],
                Normal = mesh->MNormals->Length() > 0 ? mesh->MNormals[i] : new Vector3(),
                TexCoords = new Vector2
                {
                    X = mesh->MTextureCoords.Element0->Length() > 0 ? mesh->MTextureCoords[0][i].X : 0f,
                    Y = mesh->MTextureCoords.Element0->Length() > 0 ? mesh->MTextureCoords[0][i].Y : 0f
                },
                Tangent = mesh->MTextureCoords.Element0->Length() > 0 ? mesh->MTangents[i] : new Vector3(),
                BiTangent = mesh->MTextureCoords.Element0->Length() > 0 ? mesh->MBitangents[i] : new Vector3(),
                //BonesID = stackalloc int[2];
            };
            vertices[i] = vertex;
        }
        for (int i = 0; i < mesh->MNumFaces; i++)
        {
            Face face = mesh->MFaces[i];
            // retrieve all indices of the face and store them in the indices vector
            for (int j = 0; j < face.MNumIndices; j++)
            {
                indices[i]=face.MIndices[j];
            }
        }
        // process materials
        Material* material = scene->MMaterials[mesh->MMaterialIndex];
        // we assume a convention for sampler names in the shaders. Each diffuse texture should be named
        // as 'texture_diffuseN' where N is a sequential number ranging from 1 to MAX_SAMPLER_NUMBER. 
        // Same applies to other texture as the following list summarizes:
        // diffuse: texture_diffuseN
        // specular: texture_specularN
        // normal: texture_normalN

        Span<Texture> diffuseTexture = stackalloc Texture[5];
        //for(int i = 0; i < material->GetMaterialCount(); i++)
        //{

        //}

        return new Mesh(_gl, vertices, indices, textures);
    }

    public unsafe Span<T> LoadMaterialTextures<T>(Material* mat, TextureType type,
        string typeName)
    {
        return default;
    }
}

public readonly struct VertexBufferObject
{
    public uint BufferHandle { get; }
    public BufferTargetARB BufferTargetARB { get; }

    public unsafe VertexBufferObject(GL gl, ReadOnlySpan<Vertex> span, BufferTargetARB bufferTargetARB)
    {
        //Setting the gl instance and storing our buffer type.
        BufferHandle = gl.GenBuffer();
        BufferTargetARB = bufferTargetARB;
        BindBy(gl);
        fixed (void* data = span)
        {
            gl.BufferData(BufferTargetARB,
                (nuint)(span.Length * sizeof(Vertex)),
                data,
                BufferUsageARB.StaticDraw);
        }
    }
    public void BindBy(GL gl)
    {
        //Binding the buffer object, with the correct buffer type.
        gl.BindBuffer(BufferTargetARB, BufferHandle);
    }

    private void OnDispose(GL gl)
    {
        gl.DeleteBuffer(BufferHandle);
    }

    public void DisposeBy(GL gl)
    {
        OnDispose(gl);
    }
}
