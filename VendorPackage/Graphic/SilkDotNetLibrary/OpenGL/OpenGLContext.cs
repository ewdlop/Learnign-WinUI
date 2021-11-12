using SilkDotNetLibrary.OpenGL.Shaders;
using Serilog;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using SilkDotNetLibrary.OpenGL.Buffers;
using Shader = SilkDotNetLibrary.OpenGL.Shaders.Shader;

namespace SilkDotNetLibrary.OpenGL;

public class OpenGLContext : IOpenGLContext
{
    private readonly IWindow _window;
    private bool disposedValue;
    private GL GL { get; set; }
    public BufferObject<float> Vbo { get; private set; }
    public BufferObject<uint> Ebo { get; private set; }
    public VertexArrayBufferObject<float, uint> Vao { get; private set; }
    public Shader Shader { get; private set; }

    public OpenGLContext(IWindow Window)
    {
        Log.Information("Creating OpenGLContext...");
        _window = Window;
    }

    public unsafe void OnLoad()
    {
        disposedValue = false;
        GL = GL.GetApi(_window);
        Ebo = new BufferObject<uint>(GL, Quad.Indices, BufferTargetARB.ElementArrayBuffer);
        Vbo = new BufferObject<float>(GL, Quad.Vertices, BufferTargetARB.ArrayBuffer);
        Vao = new VertexArrayBufferObject<float, uint>(GL, Vbo, Ebo);

        //Telling the VAO object how to lay out the attribute pointers
        Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 7, 0);
        Vao.VertexAttributePointer(1, 4, VertexAttribPointerType.Float, 7, 3);
        Shader = new Shader(GL);
        Shader.Load("Shaders/shader.vert", "Shaders/shader.frag");
    }

    public unsafe void OnRender(double dt)
    {
        GL.Clear((uint)ClearBufferMask.ColorBufferBit);
        Vao.Bind();
        Shader.Use();
        //Setting a uniform.
        Shader.SetUniform("uBlue", (float)Math.Sin(DateTime.Now.Millisecond / 1000f * Math.PI));
        GL.DrawElements(PrimitiveType.Triangles,
                        (uint)Quad.Indices.Length,
                        DrawElementsType.UnsignedInt,
                        null);
    }

    public void OnUpdate(double dt)
    {

    }

    public void OnDipose()
    {
        Log.Information("OpnGLContext Diposing...");
        Vbo.Dispose();
        Ebo.Dispose();
        Vao.Dispose();
        Shader.Dispose();
    }

    public void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                OnDipose();
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
