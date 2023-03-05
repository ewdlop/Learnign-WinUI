using Microsoft.Extensions.Logging;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Texture = SilkDotNetLibrary.OpenGL.Textures.Texture;

namespace SilkDotNetLibrary.OpenGL.Meshes;

public class MeshComponentFactory
{
    private readonly Assimp _assimp;
    private ILogger<MeshComponentFactory> _logger;
    //private Dictionary<uint, MeshComponent> _meshTextureInfo;
    public MeshComponentFactory(ILogger<MeshComponentFactory> logger)
    {
        _assimp = Assimp.GetApi();
        _logger = logger;
    }

    public unsafe MeshComponent LoadModel(GL gl, string path)
    {

        Scene* scene = _assimp.ImportFile(path, Convert.ToUInt32(PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs));
        if (scene is null ||
            Convert.ToBoolean(scene->MFlags & Convert.ToUInt32(SceneFlags.Incomplete)) ||
            scene->MRootNode is null)
        {
            _logger.LogError(_assimp.GetErrorStringS());
            throw new ApplicationException(_assimp.GetErrorStringS());
        }

        List<(Texture, string)> loadedTexture = new List<(Texture, string)>();
        List<(Mesh, List<Texture>)> meshes = ProcessNode(gl, ref loadedTexture, scene->MRootNode, scene);
        MeshComponent meshComponent = new()
        {
            Meshes = meshes,
            Directory = new FileInfo(path).Directory?.Parent?.ToString() ??
                        throw new FileNotFoundException("RIP", path)
        };
        return meshComponent;
    }

    private unsafe List<(Mesh, List<Texture>)> ProcessNode(GL gl, ref List<(Texture, string)> loadedTexture, Node* node, Scene* scene)
    {
        List<(Mesh, List<Texture>)> meshes = new();
        for (uint i = 0; i < node->MNumMeshes; i++)
        {
            Silk.NET.Assimp.Mesh* mesh = scene->MMeshes[node->MMeshes[i]];
            meshes.Add(ProcessMesh(gl,ref loadedTexture, mesh, scene));
        }
        for (int i = 0; i < node->MNumChildren; i++)
        {
            meshes.AddRange(ProcessNode(gl, ref loadedTexture, node->MChildren[i], scene));
        }
        return meshes;
    }

    private unsafe (Mesh, List<Texture>) ProcessMesh(GL gl, ref List<(Texture, string)> loadedTextures, Silk.NET.Assimp.Mesh* mesh, Scene* scene)
    {
        int verticesSize = (int)mesh->MNumVertices;
        int indicesSize = (int)mesh->MNumFaces;
        //Span<Vertex> vertices = stackalloc Vertex[verticesSize];//blow up the stack...
        Vertex[] vertices = new Vertex[verticesSize];//or Memory<T>?
        uint[] indices = new uint[indicesSize];
        //Span<uint> indices = stackalloc uint[indicesSize];
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

        textures.AddRange(LoadMaterialTextures(gl, ref loadedTextures, material, TextureType.Diffuse));
        textures.AddRange(LoadMaterialTextures(gl, ref loadedTextures, material, TextureType.Specular));
        textures.AddRange(LoadMaterialTextures(gl, ref loadedTextures, material, TextureType.Normals));
        textures.AddRange(LoadMaterialTextures(gl, ref loadedTextures, material, TextureType.Height));
        textures.AddRange(LoadMaterialTextures(gl, ref loadedTextures, material, TextureType.BaseColor));
        textures.AddRange(LoadMaterialTextures(gl, ref loadedTextures, material, TextureType.Metalness));
        return (new Mesh(gl, vertices, indices), textures);
    }

    private unsafe List<Texture> LoadMaterialTextures(GL gl,
        ref List<(Texture,string)> loadedTextures, 
        Material* mat, TextureType type)
    {
        bool skip = false;
        List<Texture> textures = new List<Texture>();
        for (int i = 0; i < _assimp.GetMaterialTextureCount(mat,type); i++)
        {
            AssimpString str;

            //material->GetTexture(type, i, &str);
            _assimp.GetMaterialTexture(mat,
                                       type,
                                       (uint)i,
                                       &str,
                                       mapping: null,
                                       uvindex: null,
                                       blend: null,
                                       op: null,
                                       mapmode:null,
                                       flags:null);
            // check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
            foreach ((Texture loadedTexture, string textureName) in loadedTextures)
            {
                if (!string.Equals(textureName, str.AsString)) continue;
                textures.Add(loadedTexture);
                skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
                break;
            }
            
            if (skip) continue;
            // if texture hasn't been loaded already, load it
            //hard code for the moment, relative path issue
            Image texutreImage = Image.Load($"Assets/batman_free/{str.AsString}");
            //Image texutreImage = Image.Load($"{str.AsString}");
            Texture texture = new(gl, texutreImage, type);
            textures.Add(texture);
            loadedTextures.Add((texture, str.AsString));  // store it as texture loaded for entire model, to ensure we won't unnecesery load duplicate textures.
        }
        return textures;
    }
}
