using Serilog;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using SilkDotNetLibrary.OpenGL.Buffers;
using Shader = SilkDotNetLibrary.OpenGL.Shaders.Shader;
using SilkDotNetLibrary.OpenGL.Textures;
using System.Numerics;
using SharedLibrary.Transforms;
using SharedLibrary.Math;
using SilkDotNetLibrary.OpenGL.Primitive;
using SharedLibrary.Cameras;

namespace SilkDotNetLibrary.OpenGL;

public class OpenGLContext : IOpenGLContext,IDisposable
{
    private bool disposedValue;
    private readonly IWindow _window;
    private readonly ICamera _camera;
    private readonly Transform[] Transforms = new Transform[4];
    private const int WIDTH = 800;
    private const int HEIGHT = 700;

    private GL _gl;
    private BufferObject<float> _vbo;
    private BufferObject<uint> _ebo;
    private VertexArrayBufferObject<float, uint> VaoCube { get; set; }
    private Shader Shader { get; set; }
    private Textures.Texture Texture { get; set; }

    //Setup the camera's location, and relative up and right directions
    public OpenGLContext(IWindow Window, ICamera camera)
    {
        Log.Information("Creating OpenGLContext...");
        _window = Window;
        _camera = camera;
    }

    public unsafe void OnLoad()
    {
        _gl = GL.GetApi(_window);

        //_ebo = new BufferObject<uint>(_gl, Quad.Indices, BufferTargetARB.ElementArrayBuffer);
        //_vbo = new BufferObject<float>(_gl, Quad.Vertices, BufferTargetARB.ArrayBuffer);
        _ebo = new BufferObject<uint>(_gl, Cube.Indices, BufferTargetARB.ElementArrayBuffer);
        _vbo = new BufferObject<float>(_gl, Cube.Vertices, BufferTargetARB.ArrayBuffer);
        VaoCube = new VertexArrayBufferObject<float, uint>(in _gl, in _vbo, in _ebo);

        //Telling the VAO object how to lay out the attribute pointers
        VaoCube.VertexAttributePointer(_gl, 0, 3, VertexAttribPointerType.Float, 5, 0);
        VaoCube.VertexAttributePointer(_gl, 1, 2, VertexAttribPointerType.Float, 5, 3);

        Texture = new Textures.Texture(_gl, "../../../Textures/silk.png");
        Shader = new Shader(_gl, "../../../Shaders/texture.vert", "../../../Shaders/texture.frag");
        //_shader.Load("Shaders/shader.vert", "Shaders/shader.frag");

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
        _gl.Enable(EnableCap.DepthTest);
        _gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

        VaoCube.BindBy(_gl);

        Texture.BindBy(_gl);
        Shader.UseBy(_gl);
        //Setting a uniform.
        //_shader.SetUniform("uBlue", (float)Math.Sin(DateTime.Now.Millisecond / 1000f * Math.PI));
        Shader.SetUniformBy(_gl, "uTexture0", 0);

        //for (int i = 0; i < Transforms.Length; i++)
        //{
        //    //Using the transformations.
        //    Shader.SetUniform("uModel", Transforms[i].ViewMatrix);

        //    GL.DrawElements(PrimitiveType.Triangles, (uint)DefaultTexture.Indices.Length, DrawElementsType.UnsignedInt, null);
        //}

        //Use elapsed time to convert to radians to allow our cube to rotate over time
        var difference = (float)(_window.Time * 100);

        var model = Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(difference)) * Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(difference));
        var view = Matrix4x4.CreateLookAt(_camera.Position, _camera.Position + _camera.Front, _camera.Up);
        var projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_camera.Zoom), WIDTH / HEIGHT, 0.1f, 100.0f);

        Shader.SetUniformBy(_gl,"uModel", model);
        Shader.SetUniformBy(_gl, "uView", view);
        Shader.SetUniformBy(_gl, "uProjection", projection);

        //We're drawing with just vertices and no indicies, and it takes 36 verticies to have a six-sided textured cube
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    public void OnUpdate(double dt)
    {

    }

    public void OnDispose()
    {
        Log.Information("OpnGLContext Diposing...");
        _vbo.DisposeBy(_gl);
        _ebo.DisposeBy(_gl);
        VaoCube.DisposeBy(_gl);
        Shader.DisposeBy(_gl);
        Texture.DisposeBy(_gl);
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
