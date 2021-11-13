using SilkDotNetLibrary.OpenGL.Shaders;
using Serilog;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using SilkDotNetLibrary.OpenGL.Buffers;
using Shader = SilkDotNetLibrary.OpenGL.Shaders.Shader;
using SilkDotNetLibrary.OpenGL.Textures;

namespace SilkDotNetLibrary.OpenGL;

public class OpenGLContext : IOpenGLContext
{
    private bool disposedValue;
    private readonly IWindow _window;
    private GL GL { get; set; }
    private BufferObject<float> Vbo { get; set; }
    private BufferObject<uint> Ebo { get; set; }
    private VertexArrayBufferObject<float, uint> Vao { get; set; }
    private Shader Shader { get; set; }
    private Textures.Texture Texture { get; set; }

    public OpenGLContext(IWindow Window)
    {
        Log.Information("Creating OpenGLContext...");
        _window = Window;
    }

    public unsafe void OnLoad()
    {
        GL = GL.GetApi(_window);

        //_ebo = new BufferObject<uint>(_gl, Quad.Indices, BufferTargetARB.ElementArrayBuffer);
        //_vbo = new BufferObject<float>(_gl, Quad.Vertices, BufferTargetARB.ArrayBuffer);
        Ebo = new BufferObject<uint>(GL, DefaultTexture.Indices, BufferTargetARB.ElementArrayBuffer);
        Vbo = new BufferObject<float>(GL, DefaultTexture.Vertices, BufferTargetARB.ArrayBuffer);
        Vao = new VertexArrayBufferObject<float, uint>(GL, Vbo, Ebo);
        //Telling the VAO object how to lay out the attribute pointers
        //_vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 7, 0);
        //_vao.VertexAttributePointer(1, 4, VertexAttribPointerType.Float, 7, 3);
        Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
        Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);
        Shader = new Shader(GL);
        Texture = new Textures.Texture(GL, "Textures/silk.png");
        //_shader.Load("Shaders/shader.vert", "Shaders/shader.frag");
        Shader.Load("Shaders/texture.vert", "Shaders/texture.frag");
    }

    public unsafe void OnRender(double dt)
    {
        GL.Clear((uint)ClearBufferMask.ColorBufferBit);
        Vao.Bind();
        Shader.Use();

        //Setting a uniform.
        //_shader.SetUniform("uBlue", (float)Math.Sin(DateTime.Now.Millisecond / 1000f * Math.PI));

        Texture.Bind(TextureUnit.Texture0);
        Shader.SetUniform("uTexture0", 0);

        GL.DrawElements(PrimitiveType.Triangles,
                        (uint)Quad.Indices.Length,
                        DrawElementsType.UnsignedInt,
                        null);
    }

    public void OnUpdate(double dt)
    {

    }

    public void OnDispose()
    {
        Log.Information("OpnGLContext Diposing...");
        Vbo.Dispose();
        Ebo.Dispose();
        Vao.Dispose();
        Shader.Dispose();
        Texture.Dispose();
    }

    public void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                OnDispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
        Log.Information("OpnGLContext Already Diposed...");
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~OpenGLContext()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
