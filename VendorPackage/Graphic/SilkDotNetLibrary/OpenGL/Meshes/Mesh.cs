using Serilog;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SilkDotNetLibrary.OpenGL.Buffers;
using SilkDotNetLibrary.OpenGL.Primitives;
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
    //private VertexArrayBufferObject<Vertex, uint> Vao { get; }
    private BufferObject<Vertex> Vbo { get; }
    //private BufferObject<Vertex> Vbo { get; }
    private BufferObject<uint> Ebo { get; }

    public Mesh(GL gl, ReadOnlySpan<Vertex> vertices, ReadOnlySpan<uint> indices)
    {
        IndicesLength = Convert.ToUInt32(indices.Length);
        Vbo = new BufferObject<Vertex>(gl, vertices, BufferTargetARB.ArrayBuffer);
        Ebo = new BufferObject<uint>(gl, indices, BufferTargetARB.ElementArrayBuffer);
        Vao = new VertexArrayBufferObject<Vertex, uint>(gl, Vbo, Ebo);
        
        // 只設定 shader 需要的屬性（匹配 avocado_debug.vert）
        Vao.VertexAttributePointer(gl, 0, 3, VertexAttribPointerType.Float, 1, 0);  // aPos
        Vao.VertexAttributePointer(gl, 1, 3, VertexAttribPointerType.Float, 1, Marshal.OffsetOf<Vertex>("Normal"));  // aNormal
        Vao.VertexAttributePointer(gl, 2, 2, VertexAttribPointerType.Float, 1, Marshal.OffsetOf<Vertex>("TexCoords"));  // aTexCoords
        // 注意：不設定 location 3, 4, 5 因為 shader 不需要它們
        gl.BindVertexArray(0);
    }

    //public Mesh(GL gl, ReadOnlySpan<Vertex> vertices, ReadOnlySpan<uint> indices)
    //{
    //    IndicesLength = Convert.ToUInt32(indices.Length);
    //    Vbo = new BufferObject<Vertex>(gl, vertices, BufferTargetARB.ArrayBuffer);
    //    Ebo = new BufferObject<uint>(gl, indices, BufferTargetARB.ElementArrayBuffer);
    //    Vao = new VertexArrayBufferObject<Vertex, uint>(gl, Vbo, Ebo);
    //    Vao.VertexAttributePointer(gl, 0, 3, VertexAttribPointerType.Float, 1,
    //        0);
    //    Vao.VertexAttributePointer(gl, 1, 3, VertexAttribPointerType.Float, 1,
    //        Marshal.OffsetOf(typeof(Vertex), "Normal"));
    //    Vao.VertexAttributePointer(gl, 2, 2, VertexAttribPointerType.Float, 1,
    //        Marshal.OffsetOf(typeof(Vertex), "TexCoords"));
    //    Vao.VertexAttributePointer(gl, 3, 3, VertexAttribPointerType.Float, 1,
    //       Marshal.OffsetOf(typeof(Vertex), "Tangent"));
    //    Vao.VertexAttributePointer(gl, 4, 3, VertexAttribPointerType.Float, 1,
    //        Marshal.OffsetOf(typeof(Vertex), "BiTangent"));
    //}

    public enum RenderMode
    {
        Fill,
        Wireframe,
        Points
    }

    public unsafe void RenderWireframe(GL gl, float lineWidth = 1.0f)
    {
        // Set line width
        gl.LineWidth(lineWidth);

        // Enable wireframe mode
        gl.PolygonMode(GLEnum.FrontAndBack, PolygonMode.Line);

        // Optional: Disable depth test for better wireframe visibility
        gl.Disable(EnableCap.DepthTest);

        // Bind and draw
        Vao.BindBy(gl);
        gl.DrawElements(Silk.NET.OpenGL.PrimitiveType.Triangles, IndicesLength, DrawElementsType.UnsignedInt, (void*)0);

        // Restore settings
        gl.Enable(EnableCap.DepthTest);
        gl.PolygonMode(GLEnum.FrontAndBack, PolygonMode.Fill);
        gl.LineWidth(1.0f);
        //Vao.UnBind();
    }



    public unsafe void Draw(GL gl)
    {
        //Draw the mesh without textures
        Console.WriteLine($"Drawing {IndicesLength} indices without textures");
        Console.WriteLine($"VAO bound: {gl.GetInteger(GLEnum.VertexArrayBinding) != 0}");
        // Check if we're actually calling draw
        Vao.BindBy(gl);
        Console.WriteLine($"VAO bound: {gl.GetInteger(GLEnum.VertexArrayBinding) != 0}");
        Console.WriteLine(
            "About to call DrawElements...");
        gl.DrawElements(Silk.NET.OpenGL.PrimitiveType.Triangles, IndicesLength, DrawElementsType.UnsignedInt, (void*)0);
        Console.WriteLine(
            "DrawElements called.");
#if DEBUG
        var error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Console.WriteLine($"OpenGL Error after draw: {error}");
        }
#endif
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
        gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, "num_diffuse_textures"), (int)diffuseNr);
#if DEBUG
        var error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Log.Error($"gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, \"num_diffuse_textures\"), {diffuseNr});");
            Log.Error("OpenGL Error: {Error}", error);
        }
#endif
        //uniform int num_normal_textures;
        gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, "num_normal_textures"), (int)normalNr);
#if DEBUG
        error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Log.Error("gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, \"num_normal_textures\"), normalNr);");
            Log.Error("OpenGL Error: {Error}", error);
        }
#endif
        //uniform int num_specular_textures;
        gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, "num_specular_textures"), (int)specularNr);
#if DEBUG
        error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Log.Error("gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, \"num_specular_textures\"), specularNr);");
            Log.Error("OpenGL Error: {Error}", error);
        }
#endif
        //uniform int num_height_textures;
        gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, "num_height_textures"), (int)heightNr);
#if DEBUG
        error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Log.Error("gl.Uniform1(gl.GetUniformLocation(shader.ShaderProgramHandle, \"num_height_textures\"), heightNr);");
            Log.Error("OpenGL Error: {Error}", error);
        }
#endif
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
