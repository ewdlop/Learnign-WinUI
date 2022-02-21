using System;
using System.IO;
using System.Numerics;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SilkDotNetLibrary.OpenGL.Buffers;
using SilkDotNetLibrary.OpenGL.Primitive;
using Shader = SilkDotNetLibrary.OpenGL.Shaders.Shader;

namespace SilkDotNetLibrary.Model;

public struct Vertex
{
    Vector3 Position;
    Vector3 Normal;
    Vector2 TexCoords;
}

public struct Texture
{
    uint id;
    string type;
}

public struct Mesh
{
    //private Vertex[] Vertices { get; }
    //private uint[] Indices { get; }
    //private Texture[] Textures { get; }
    VertexArrayBufferObject<Vertex,uint> Vao { get; }
    BufferObject<Vertex> Vbo { get; }
    BufferObject<uint> Ebo { get;}

    public Mesh(GL gl, ReadOnlySpan<Vertex> vertices, ReadOnlySpan<uint> indices, ReadOnlySpan<Texture> textures)
    {
        Vbo = new BufferObject<Vertex>(gl, vertices, BufferTargetARB.ArrayBuffer);
        Ebo = new BufferObject<uint>(gl, indices, BufferTargetARB.ElementArrayBuffer);
        Vao = new VertexArrayBufferObject<Vertex, uint>(gl, Vbo, Ebo);
    }
    private void SetupMesh()
    {

    }

 
}
public  class AssimpContext
{

    private readonly Assimp _assimp;

    public AssimpContext()
    {
        _assimp = Assimp.GetApi();
    }

    public unsafe void LoadModel(string path)
    {
        Scene* scene = _assimp.ImportFile(path, (uint) (PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs));
        if (scene is not null || 
            Convert.ToBoolean(scene->MFlags & (uint) SceneFlags.Incomplete) ||
            scene->MRootNode is not null)
        {
            Console.Write($"ERROR::ASSIMP:{_assimp.GetErrorStringS()}");
            return;
        }
        //if (!scene || scene->mFlags & AI_SCENE_FLAGS_INCOMPLETE || !scene->mRootNode)
        //{
        //    cout << "ERROR::ASSIMP::" << import.GetErrorString() << endl;
        //    return;
        //}
        //directory = path.substr(0, path.find_last_of('/'));

        ProcessNode(scene->MRootNode, scene);
    }
    public void Draw(Shader shader)
    {

    }

    public unsafe void ProcessNode(Node* node, Scene* scene)
    {

    }
    public unsafe Mesh ProcessMesh(Mesh* mesh,  Scene* scene)
    {
        return default;
    }

    public unsafe Texture[] LoadMaterialTextures(Material* mat, TextureType type,
        string typeName)
    {
        return default;
    }
}