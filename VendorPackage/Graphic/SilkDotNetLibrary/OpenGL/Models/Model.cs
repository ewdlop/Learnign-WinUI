using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SilkDotNetLibrary.OpenGL.Buffers;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SilkDotNetLibrary.OpenGL.Models;

public unsafe struct Vertex
{
    public Vector3 Position { get; init; }
    public Vector3 Normal { get; init; }
    public Vector2 TexCoords { get; init; }
    public Vector3 Tangent { get; init; }
    public Vector3 BiTangent { get; init; }

    //public Span<int> BonesID = stackalloc int[5];
}

public struct Texture
{
    public uint id;
}

public ref struct Test
{

}
public record struct Mesh
{
    //private Vertex[] Vertices { get; }
    //private uint[] Indices { get; }
    //private Texture[] Textures { get; }

    private VertexArrayBufferObject<Vertex, uint> Vao { get; }
    private BufferObject<Vertex> Vbo { get; }
    private BufferObject<uint> Ebo { get; }


    public unsafe Mesh(GL gl, ReadOnlySpan<Vertex> vertices, ReadOnlySpan<uint> indices, ReadOnlySpan<Texture> textures)
    {
        Test* test = stackalloc Test[2];

        Vbo = new BufferObject<Vertex>(gl, vertices, BufferTargetARB.ArrayBuffer);
        Ebo = new BufferObject<uint>(gl, indices, BufferTargetARB.ElementArrayBuffer);
        Vao = new VertexArrayBufferObject<Vertex, uint>(gl, Vbo, Ebo);
        Vao.VertexAttributePointer(gl, 0, 3, VertexAttribPointerType.Float, 1, 0);
        Vao.VertexAttributePointer(gl, 1, 3, VertexAttribPointerType.Float, 1,
            Marshal.OffsetOf(typeof(Vertex), "Normal").ToInt32());
        Vao.VertexAttributePointer(gl, 2, 2, VertexAttribPointerType.Float, 1,
            Marshal.OffsetOf(typeof(Vertex), "TexCoords").ToInt32());
    }

    public void Draw(SilkDotNetLibrary.OpenGL.Shaders.Shader shader, ReadOnlySpan<Texture> textures)
    {
        uint diffuseNr = 1;
        uint specularNr = 1;
        for (uint i = 0; i < textures.Length; i++)
        {

        }
    }
}

//public static class X
//{
//    public static T[] ToAppendedArray<T>(this Span<T> array, T element) where T : unmanaged
//    {
//        Span<T> newArray = stackalloc T[array.Length + 1];
//        array.CopyTo(newArray);
//        newArray[array.Length] = element;
//        return newArray.ToArray();
//    }
//}


public class Model
{
    private readonly GL _gl;
    private readonly Assimp _assimp;
    public string Directory { get; private set; }
    public bool GammaCoorection { get; private set; }
    public Model(GL gl, bool gammaCoorection = false)
    {
        _gl = gl;
        _assimp = Assimp.GetApi();
        GammaCoorection = gammaCoorection;
    }

    public unsafe void LoadModel(string path)
    {
        Scene* scene = _assimp.ImportFile(path, Convert.ToUInt32(PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs));
        if (scene is not null ||
            Convert.ToBoolean(scene->MFlags & Convert.ToUInt32(SceneFlags.Incomplete)) ||
            scene->MRootNode is not null)
        {
            Console.Write($"ERROR::ASSIMP:{_assimp.GetErrorStringS()}");
            return;
        }

        Directory = new System.IO.FileInfo(path).Directory?.Parent?.ToString() ?? throw new FileNotFoundException("RIP", path);

        ProcessNode(scene->MRootNode, scene);
    }
    public void Draw(SilkDotNetLibrary.OpenGL.Shaders.Shader shader)
    {

    }

    public unsafe void ProcessNode(Node* node, Scene* scene)
    {
        for (uint i = 0; i < node->MNumMeshes; i++)
        {
            Silk.NET.Assimp.Mesh* mesh = scene->MMeshes[node->MMeshes[i]];
            //meshes.push_back(processMesh(mesh, scene));
        }
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
        return new Mesh(_gl, vertices, indices, textures);
    }

    public unsafe Texture[] LoadMaterialTextures(Material* mat, TextureType type,
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
