using Microsoft.Extensions.Logging;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Texture = SilkDotNetLibrary.OpenGL.Textures.Texture;

namespace SilkDotNetLibrary.OpenGL.Meshes;

public class MeshComponentFactory(ILogger<MeshComponentFactory> logger)
{
    private readonly Assimp _assimp = Assimp.GetApi();
    private readonly ILogger<MeshComponentFactory> _logger = logger;

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
        _logger.LogInformation("Model loaded: {Path}", path);

        List<(Texture, string)> loadedTexture = [];
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
        List<(Mesh, List<Texture>)> meshes = [];
        for (uint i = 0; i < node->MNumMeshes; i++)
        {
            //Process each mesh located at the node
            //and append it to the list of meshes

            Silk.NET.Assimp.Mesh* mesh = scene->MMeshes[node->MMeshes[i]];
            meshes.Add(ProcessMesh(gl,ref loadedTexture, mesh, scene));

            _logger.LogInformation("Mesh loaded: {Name}", mesh->MName);
        }
        for (int i = 0; i < node->MNumChildren; i++)
        {
            meshes.AddRange(ProcessNode(gl, ref loadedTexture, node->MChildren[i], scene));

            _logger.LogInformation("Node loaded: {Name}", node->MChildren[i]->MName);
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
        List<Texture> textures = [];

        // process vertex data
        _logger.LogInformation("Mesh vertices: {Vertices}", verticesSize);
        _logger.LogInformation("Mesh indices: {Indices}", indicesSize);

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

    //private unsafe List<Texture> LoadMaterialTextures(GL gl,
    //    ref List<(Texture,string)> loadedTextures, 
    //    Material* mat, TextureType type)
    //{
    //    _logger.LogInformation("Material type: {Type}", type);
    //    bool skip = false;
    //    List<Texture> textures = [];
    //    for (int i = 0; i < _assimp.GetMaterialTextureCount(mat,type); i++)
    //    {
    //        AssimpString str;

    //        //material->GetTexture(type, i, &str);
    //        _assimp.GetMaterialTexture(mat,
    //                                   type,
    //                                   (uint)i,
    //                                   &str,
    //                                   mapping: null,
    //                                   uvindex: null,
    //                                   blend: null,
    //                                   op: null,
    //                                   mapmode:null,
    //                                   flags:null);
    //        _logger.LogInformation("Texture path: {Path}", str.AsString);

    //        // check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
    //        foreach ((Texture loadedTexture, string textureName) in loadedTextures)
    //        {
    //            if (!string.Equals(textureName, str.AsString)) continue;
    //            textures.Add(loadedTexture);
    //            skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
    //            _logger.LogInformation("Texture loaded: {AssimpString}", str.AsString);
    //            break;
    //        }

    //        if (skip)
    //        {
    //            _logger.LogInformation("Texture already loaded: {AssimpString}", str.AsString);
    //            continue;
    //        }
    //        // if texture hasn't been loaded already, load it
    //        //hard code for the moment, relative path issue
            
    //        Image texutreImage = Image.Load($"Assets/batman_free/{str.AsString}");

    //        _logger.LogInformation("Height: {Height}", texutreImage.Height);
    //        _logger.LogInformation("Width: {PixelType}", texutreImage.Width);
    //        _logger.LogInformation("PixelType: {PixelType}", texutreImage.PixelType);
    //        _logger.LogInformation("BitsPerPixel: {TextureImage}", texutreImage.PixelType.BitsPerPixel);
    //        _logger.LogInformation("AlphaRepresentation: {AlphaRepresentation}", texutreImage.PixelType.AlphaRepresentation);

    //        //Image texutreImage = Image.Load($"{str.AsString}");
    //        Texture texture = new(gl, texutreImage, type);
    //        textures.Add(texture);
    //        loadedTextures.Add((texture, str.AsString));  // store it as texture loaded for entire model, to ensure we won't unnecesery load duplicate textures.
    //    }
    //    return textures;
    //}

    private unsafe List<Texture> LoadMaterialTextures(GL gl,
    ref List<(Texture, string)> loadedTextures,
    Material* mat, TextureType type)
    {
        _logger.LogInformation("Processing material type: {Type}", type);

        //// Validate texture type
        //if (!Enum.IsDefined(typeof(TextureType), type))
        //{
        //    _logger.LogWarning("Unsupported texture type: {Type}", type);
        //    return new List<Texture>();
        //}

        uint textureCount = _assimp.GetMaterialTextureCount(mat, type);
        if(textureCount == 0)
        {
            _logger.LogInformation("No textures found for material type: {Type}", type);
            return new List<Texture>();
        }

        List<Texture> textures = new();
        for (int i = 0; i < _assimp.GetMaterialTextureCount(mat, type); i++)
        {
            AssimpString str;

            // Get the texture path
            _assimp.GetMaterialTexture(mat,
                                       type,
                                       (uint)i,
                                       &str,
                                       mapping: null,
                                       uvindex: null,
                                       blend: null,
                                       op: null,
                                       mapmode: null,
                                       flags: null);

            string texturePath = str.AsString;
            _logger.LogInformation("Texture path: {Path}", texturePath);

            // Validate texture path
            if (string.IsNullOrWhiteSpace(texturePath))
            {
                _logger.LogWarning("Texture path is invalid or empty.");
                continue;
            }

            string fullPath = Path.Combine("Assets/avocado", texturePath);
            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogWarning("Texture file not found: {FullPath}", fullPath);
                continue;
            }

            // Check if texture is already loaded
            (Texture, string) existingTexture = loadedTextures.FirstOrDefault(t => t.Item2 == texturePath);
            if (!EqualityComparer<(Texture, string)>.Default.Equals(existingTexture, default))
            {
                _logger.LogInformation("Texture already loaded: {Path}", texturePath);
                textures.Add(existingTexture.Item1);
                continue;
            }

            // Load the texture
            try
            {
                //metalic texture texture load issue
                using Image textureImage = Image.Load(fullPath);
                _logger.LogInformation("Loaded texture: {Path}, Width: {Width}, Height: {Height}",
                    texturePath, textureImage.Width, textureImage.Height);

                Texture texture = new(gl, textureImage, type);
                textures.Add(texture);
                loadedTextures.Add((texture, texturePath)); // Track the loaded texture
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load texture: {Path}", texturePath);
            }
        }

        return textures;
    }
}
