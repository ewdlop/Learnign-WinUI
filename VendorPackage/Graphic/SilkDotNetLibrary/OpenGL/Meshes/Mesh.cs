using Serilog;
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
        uint diffuseNr = 0;
        uint specularNr = 0;
        uint normalNr = 0;
        uint heightNr = 0;
        for (int i = 0; i < textures.Count; i++)
        {
            Log.Debug("Binding texture {TextureIndex} of type {TextureType}", i, textures[i].TextureType);
            gl.ActiveTexture(GLEnum.Texture0 + i); // active proper texture unit before binding

            // retrieve texture number (the N in diffuse_textureN)

            string combined = string.Empty;
            switch (textures[i].TextureType)
            {
                case TextureType.Diffuse:
                    combined = $"texture_diffuse[{diffuseNr}]";
                    diffuseNr++;
                    break;
                case TextureType.Specular:
                    combined = $"texture_specular[{specularNr}]";
                    specularNr++;
                    break;
                case TextureType.Normals:
                    combined = $"texture_normal[{normalNr}]";
                    normalNr++;
                    break;
                case TextureType.Height:
                    combined = $"texture_height[{heightNr}]";
                    heightNr++;
                    break;
                default:
                    break;
            }

            // now set the sampler to the correct texture unit
            gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, combined), i);
#if DEBUG
            var error2 = gl.GetError();
            if (error2 != GLEnum.NoError)
            {
                Log.Error($"gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, {combined}), i);");
                Log.Error("OpenGL Error: {Error}", error2);
            }
#endif
            // and finally bind the texture
            gl.BindTexture(GLEnum.Texture2D, textures[i].TextureHandle);
#if DEBUG
            error2 = gl.GetError();
            if (error2 != GLEnum.NoError)
            {
                Log.Error("gl.BindTexture(GLEnum.Texture2D, textures[i].TextureHandle);;");
                Log.Error("OpenGL Error: {Error}", error2);
            }
#endif
        }
        //uniform int num_diffuse_textures;
        //gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, "num_diffuse_textures"), diffuseNr);
#if DEBUG
        var error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Log.Error($"gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, \"num_diffuse_textures\"), {diffuseNr});");
            Log.Error("OpenGL Error: {Error}", error);
        }
#endif
        //uniform int num_normal_textures;
        //gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, "num_normal_textures"), normalNr);
        error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Log.Error("gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, \"num_normal_textures\"), normalNr);");
            Log.Error("OpenGL Error: {Error}", error);
        }
        //uniform int num_specular_textures;
        //gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, "num_specular_textures"), specularNr);
#if DEBUG
        error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Log.Error("gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, \"num_specular_textures\"), specularNr);");
            Log.Error("OpenGL Error: {Error}", error);
        }
        //uniform int num_height_textures;
        //gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, "num_height_textures"), heightNr);
#endif
        error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Log.Error("gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, \"num_height_textures\"), heightNr);");
            Log.Error("OpenGL Error: {Error}", error);
        }
        // draw mesh
        Console.WriteLine($"Drawing {IndicesLength} indices");
        Console.WriteLine($"VAO bound: {gl.GetInteger(GLEnum.VertexArrayBinding) != 0}");

        // Check if we're actually calling draw
        Vao.BindBy(gl);
        Console.WriteLine($"VAO bound: {gl.GetInteger(GLEnum.VertexArrayBinding) != 0}");

        Console.WriteLine("About to call DrawElements...");
        gl.DrawElements(GLEnum.Triangles, IndicesLength, GLEnum.UnsignedInt, 0);
        Console.WriteLine("DrawElements called.");

        // Check for OpenGL errors
#if DEBUG
         error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Console.WriteLine($"OpenGL Error after draw: {error}");
        }
#endif
        error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Log.Error($"gl.DrawElements(GLEnum.Triangles, IndicesLength, GLEnum.UnsignedInt, 0);");
            Log.Error("OpenGL Error: {Error}", error);
        }
        gl.BindVertexArray(0);
#if DEBUG
        error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Log.Error($"gl.BindVertexArray(0);");
            Log.Error("OpenGL Error: {Error}", error);
        }
#endif
        // always good practice to set everything back to defaults once configured.
        gl.ActiveTexture(GLEnum.Texture0);
#if DEBUG
        error = gl.GetError();
        if (error != GLEnum.NoError)
        {

            Log.Error($"gl.ActiveTexture(GLEnum.Texture0);");
            Log.Error("OpenGL Error: {Error}", error);
        }
#endif
    }
}
