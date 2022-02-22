using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SilkDotNetLibrary.OpenGL.Buffers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Texture = SilkDotNetLibrary.OpenGL.Textures.Texture;

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

public record struct Mesh
{
    //private Vertex[] Vertices { get; }
    //private uint[] Indices { get; }
    //private Texture[] Textures { get; }

    private uint IndicesLength { get; }
    private VertexArrayBufferObject<Vertex, uint> Vao { get; }
    private BufferObject<Vertex> Vbo { get; }
    private BufferObject<uint> Ebo { get; }

    public Mesh(GL gl, ReadOnlySpan<Vertex> vertices, ReadOnlySpan<uint> indices)
    {
        IndicesLength = Convert.ToUInt32(indices.Length);
        Vbo = new BufferObject<Vertex>(gl, vertices, BufferTargetARB.ArrayBuffer);
        Ebo = new BufferObject<uint>(gl, indices, BufferTargetARB.ElementArrayBuffer);
        Vao = new VertexArrayBufferObject<Vertex, uint>(gl, Vbo, Ebo);
        Vao.VertexAttributePointer(gl, 0, 3, VertexAttribPointerType.Float, 1, 
            0);
        Vao.VertexAttributePointer(gl, 1, 3, VertexAttribPointerType.Float, 1,
            Marshal.OffsetOf(typeof(Vertex), "Normal").ToInt32());
        Vao.VertexAttributePointer(gl, 2, 2, VertexAttribPointerType.Float, 1,
            Marshal.OffsetOf(typeof(Vertex), "TexCoords").ToInt32());
        Vao.VertexAttributePointer(gl, 3, 3, VertexAttribPointerType.Float, 1,
            Marshal.OffsetOf(typeof(Vertex), "Tangent").ToInt32());
        Vao.VertexAttributePointer(gl, 4, 3, VertexAttribPointerType.Float, 1,
            Marshal.OffsetOf(typeof(Vertex), "BiTangent").ToInt32());
    }

    public void Draw(GL gl, SilkDotNetLibrary.OpenGL.Shaders.Shader shader, List<Texture> textures)
    {
        uint diffuseNr = 1;
        uint specularNr = 1;
        uint normalNr = 1;
        uint heightNr = 1;
        for (int i = 0; i < textures.Count; i++)
        {
            gl.ActiveTexture(GLEnum.Texture0 + i); // active proper texture unit before binding
            
            // retrieve texture number (the N in diffuse_textureN)

            string combined = textures[i].TextureType switch
            {
                TextureType.TextureTypeDiffuse => $"texture_diffuse{ diffuseNr++}",
                TextureType.TextureTypeSpecular => $"texture_specular{specularNr++}",
                TextureType.TextureTypeNormals => $"texture_normal{normalNr++}",
                TextureType.TextureTypeHeight => $"texture_height{heightNr++}",
                _ => string.Empty
            };

            // now set the sampler to the correct texture unit
            gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, combined), i);
            // and finally bind the texture
            gl.BindTexture(GLEnum.Texture2D, textures[i].TextureHandle);
        }
        // draw mesh
        Vao.BindBy(gl);
        gl.DrawElements(GLEnum.Triangles, IndicesLength, GLEnum.UnsignedInt, 0);
        gl.BindVertexArray(0);
        // always good practice to set everything back to defaults once configured.
        gl.ActiveTexture(GLEnum.Texture0);
    }
}

public class MeshComponent//model is an entity?
{
    public List<(Mesh, List<Texture>)> Meshes { get; init; }
    //public Texture[] Textures { get; init; }
    public string Directory { get; init; }
    public bool GammaCoorection { get; init; }
}
public class MeshComponentFactory
{
    private readonly GL _gl;
    private readonly Assimp _assimp;
    //private Dictionary<uint, MeshComponent> _meshTextureInfo;
    public MeshComponentFactory(GL gl)
    {
        _gl = gl;
        _assimp = Assimp.GetApi();
    }

    public unsafe MeshComponent LoadModel(string path)
    {
        Scene* scene = _assimp.ImportFile(path, Convert.ToUInt32(PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs));
        if (scene is not null ||
            Convert.ToBoolean(scene->MFlags & Convert.ToUInt32(SceneFlags.Incomplete)) ||
            scene->MRootNode is not null)
        {
            Console.Write($"ERROR::ASSIMP:{_assimp.GetErrorStringS()}");
            return null;
        }

        List<(Texture, string)> loadedTexture = new List<(Texture, string)>();
        List<(Mesh, List<Texture>)> meshes = ProcessNode(loadedTexture, scene->MRootNode, scene);
        MeshComponent meshComponent = new()
        {
            Meshes = meshes,
            Directory = new FileInfo(path).Directory?.Parent?.ToString() ??
                        throw new FileNotFoundException("RIP", path)
        };
        return meshComponent;
    }

    public unsafe List<(Mesh, List<Texture>)> ProcessNode(List<(Texture, string)> loadedTexture, Node* node, Scene* scene)
    {
        List<(Mesh, List<Texture>)> meshes = new();
        for (uint i = 0; i < node->MNumMeshes; i++)
        {
            Silk.NET.Assimp.Mesh* mesh = scene->MMeshes[node->MMeshes[i]];
            meshes.Add(ProcessMesh(loadedTexture, mesh, scene));
        }
        for (int i = 0; i < node->MNumChildren; i++)
        {
            meshes.AddRange(ProcessNode(loadedTexture, node->MChildren[i], scene));
        }
        return meshes;
    }

    public unsafe (Mesh, List<Texture>) ProcessMesh(List<(Texture, string)> loadedTexture, Silk.NET.Assimp.Mesh* mesh, Scene* scene)
    {
        int verticesSize = (int)mesh->MNumVertices;
        int indicesSize = (int)mesh->MNumFaces;
        Span<Vertex> vertices = stackalloc Vertex[verticesSize];
        Span<uint> indices = stackalloc uint[indicesSize];
        List<Texture> textures = new();
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
                BiTangent = mesh->MTextureCoords.Element0->Length() > 0 ? mesh->MBitangents[i] : new Vector3()
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

        textures.AddRange(LoadMaterialTextures(loadedTexture, material, TextureType.TextureTypeDiffuse));
        textures.AddRange(LoadMaterialTextures(loadedTexture, material, TextureType.TextureTypeSpecular));
        textures.AddRange(LoadMaterialTextures(loadedTexture, material, TextureType.TextureTypeNormals));
        textures.AddRange(LoadMaterialTextures(loadedTexture, material, TextureType.TextureTypeHeight));

        return (new Mesh(_gl, vertices, indices), textures);
    }

    public unsafe List<Texture> LoadMaterialTextures(
        List<(Texture,string)> loadedTextures, 
        Material* mat, TextureType type)
    {
        bool skip = false;
        List<Texture> textures = new List<Texture>();
        for (int i = 0; i < 10; i++)//material->GetMaterialCount()
        {
            AssimpString* str = null;
            
            //material->GetTexture(type, i, &str);
            
            // check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
            foreach ((Texture, string) loadedTexture in loadedTextures)
            {
                if (string.CompareOrdinal(loadedTexture.Item2, str->AsString) != 0) continue;
                textures.Add(loadedTexture.Item1);
                skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
                break;
            }
            
            if (skip) continue;
            // if texture hasn't been loaded already, load it
            Texture texture = new(_gl, str->AsString, type);
            textures.Add(texture);
            loadedTextures.Add((texture, str->AsString));  // store it as texture loaded for entire model, to ensure we won't unnecesery load duplicate textures.
        }
        return textures;
    }
}

public static class MeshExensions
{
    public static (List<T1>, List<T2>) AddRange<T1, T2>(this (List<T1>, List<T2>) x, (List<T1>, List<T2>) y)
    {
        x.Item1.AddRange(y.Item1);
        x.Item2.AddRange(y.Item2);
        return x;
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

//whhhyyy??????????????
//public static class X<T> where T : unmanaged
//{
//    public static T[] ToAppendedArray<T>(this ReadOnlySpan<T> array, T element)
//    {
//        Span<T> newArray = stackalloc T[array.Length + 1];
//        array.CopyTo(newArray);
//        newArray[array.Length] = element;
//        return newArray.ToArray();
//    }
//}
public static class X//<T>where T : unmanaged
{
    public static T[] ToAppendedArray<T>(this ReadOnlySpan<T> array, T element) where T : unmanaged
    {
        Span<T> newArray = stackalloc T[array.Length + 1];
        array.CopyTo(newArray);
        newArray[array.Length] = element;
        return newArray.ToArray();
    }
    public static T[] ConvertToAppendedArray<T>(ReadOnlySpan<T> array, ReadOnlySpan<T> element) where T : unmanaged
    {
        Span<T> newArray = stackalloc T[array.Length + element.Length];
        array.CopyTo(newArray);
        for (int i = 0; i < element.Length; i++)
        {
            newArray[i + array.Length] = element[i];
        }
        return newArray.ToArray();
    }
    //public static T[] ToAppendedArray2<T>(T[] array, T[] element) where T : unmanaged
    //{
    //    return ConvertToAppendedArray(array.AsSpan(), element.AsSpan());
    //}
    public static void a(out Span<int> x)
    {
        x = new Span<int>(new int[]{1});
    }
}