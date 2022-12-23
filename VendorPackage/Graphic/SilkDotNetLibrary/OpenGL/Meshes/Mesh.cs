using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SilkDotNetLibrary.OpenGL.Buffers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Texture = SilkDotNetLibrary.OpenGL.Textures.Texture;

namespace SilkDotNetLibrary.OpenGL.Meshes;

public readonly record struct Mesh
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
                TextureType.Diffuse => $"texture_diffuse{diffuseNr++}",
                TextureType.Specular => $"texture_specular{specularNr++}",
                TextureType.Normals => $"texture_normal{normalNr++}",
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
