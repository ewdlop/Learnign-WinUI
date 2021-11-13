using Serilog;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using SilkDotNetLibrary.OpenGL.Buffers;
using Shader = SilkDotNetLibrary.OpenGL.Shaders.Shader;
using SilkDotNetLibrary.OpenGL.Textures;
using SilkDotNetLibrary.Transforms;
using System.Numerics;

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
    private Transform[] Transforms { get; set; } = new Transform[4];

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

        //Unlike in the transformation, because of our abstraction, order doesn't matter here.
        //Translation.
        Transforms[0] = new();
        Transforms[0].Position = new Vector3(0.5f, 0.5f, 0f);
        //Rotation.
        Transforms[1] = new();
        Transforms[1].Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 1f);
        //Scaling.
        Transforms[2] = new();
        Transforms[2].Scale = 0.5f;
        //Mixed transformation.
        Transforms[3] = new();
        Transforms[3].Position = new Vector3(-0.5f, 0.5f, 0f);
        Transforms[3].Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 1f);
        Transforms[3].Scale = 0.5f;
    }

    public unsafe void OnRender(double dt)
    {
        GL.Clear((uint)ClearBufferMask.ColorBufferBit);
        Vao.Bind();
        Texture.Bind();
        Shader.Use();
        //Setting a uniform.
        //_shader.SetUniform("uBlue", (float)Math.Sin(DateTime.Now.Millisecond / 1000f * Math.PI));
        Shader.SetUniform("uTexture0", 0);
        for (int i = 0; i < Transforms.Length; i++)
        {
            //Using the transformations.
            Shader.SetUniform("uModel", Transforms[i].ViewMatrix);

            GL.DrawElements(PrimitiveType.Triangles, (uint)DefaultTexture.Indices.Length, DrawElementsType.UnsignedInt, null);
        }
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
