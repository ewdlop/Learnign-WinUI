using Microsoft.Extensions.Logging;
using SharedLibrary.Cameras;
using SharedLibrary.Math;
using SharedLibrary.Transforms;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SilkDotNetLibrary.OpenGL.Buffers;
using SilkDotNetLibrary.OpenGL.Primitive;
using System;
using System.Numerics;
using SharedLibrary.Systems;
using Shader = SilkDotNetLibrary.OpenGL.Shaders.Shader;

namespace SilkDotNetLibrary.OpenGL;

public class OpenGLContext : IOpenGLContext, IDisposable
{
    private bool _disposedValue;
    private readonly IWindow _window;
    private readonly ICamera _camera;
    private readonly ILogger<OpenGLContext> _logger;
    private readonly Transform[] _transforms = new Transform[4];
    private readonly Vector3 _lampPosition = new(1.2f, 1.0f, 2.0f);
    private GL _gl;
    private BufferObject<float> _vbo;
    private BufferObject<uint> _ebo;
    //private ImGuiController _ImGuiController;
    private VertexArrayBufferObject<float, uint> VaoCube { get; set; }
    private Shader LightingShader { get; set; }
    private Shader LampShader { get; set; }
    //private Textures.Texture Texture { get; set; }
    private Textures.Texture DiffuseMap { get; set; }
    private Textures.Texture SpecularMap { get; set; }
    private float Time { get; set; }

    //Setup the camera's location, and relative up and right directions
    public OpenGLContext(IWindow window, ICamera camera, ILogger<OpenGLContext> logger)
    {
        _window = window;
        _camera = camera;
        _logger = logger;
        _logger.LogInformation("Creating OpenGLContext...");
    }

    public GL OnLoad()
    {
        _gl = _window.CreateOpenGL();
        _ebo = new BufferObject<uint>(_gl, TexturedNormaledCube.Indices, BufferTargetARB.ElementArrayBuffer);
        _vbo = new BufferObject<float>(_gl, TexturedNormaledCube.Vertices, BufferTargetARB.ArrayBuffer);
        VaoCube = new VertexArrayBufferObject<float, uint>(_gl, _vbo, _ebo);

        //Telling the VAO object how to lay out the attribute pointers
        VaoCube.VertexAttributePointer(_gl, 0, 3, VertexAttribPointerType.Float, TexturedNormaledCube.VerticeSize, 0);
        VaoCube.VertexAttributePointer(_gl, 1, 3, VertexAttribPointerType.Float, TexturedNormaledCube.VerticeSize, 3);
        VaoCube.VertexAttributePointer(_gl, 2, 2, VertexAttribPointerType.Float, TexturedNormaledCube.VerticeSize, 6);

        LightingShader = new Shader(_gl, "Shaders/Raw/basic.vert", "Shaders/Raw/material.frag");
        //The Lamp shader uses a fragment shader that just colors it solid white so that we know it is the light source
        LampShader = new Shader(_gl, "Shaders/Raw/basic.vert", "Shaders/Raw/white.frag"); ;
        DiffuseMap = new Textures.Texture(_gl, "Textures/Raw/silkBoxed.png");
        SpecularMap = new Textures.Texture(_gl, "Textures/Raw/silkSpecular.png");

        return _gl;
    }

    public void OnRender(double dt)
    {
        Time += (float)dt;
        _gl.Enable(EnableCap.DepthTest);
        _gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

        VaoCube.BindBy(_gl);
        DiffuseMap.BindBy(_gl, TextureUnit.Texture0);
        SpecularMap.BindBy(_gl, TextureUnit.Texture1);


        var difference = (float)(_window.Time * 100);

        //Slightly rotate the cube to give it an angled face to look at
        LightingShader.UseBy(_gl);

        LightingShader.SetUniformBy(_gl, "uModel", Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(difference)) * Matrix4x4.CreateTranslation(new Vector3(0f,-1*Time,0f)));
        LightingShader.SetUniformBy(_gl, "uView", _camera.GetViewMatrix());
        LightingShader.SetUniformBy(_gl, "uProjection", _camera.GetProjectionMatrix());
        LightingShader.SetUniformBy(_gl, "viewPos", _camera.Position);
        LightingShader.SetUniformBy(_gl, "material.diffuse", 0);
        //Specular is set to 1 because our diffuseMap is bound to Texture1
        LightingShader.SetUniformBy(_gl, "material.specular", 1);
        LightingShader.SetUniformBy(_gl, "material.shininess", 32.0f);


        var diffuseColor = new Vector3(0.5f);
        var ambientColor = diffuseColor * new Vector3(0.2f);

        LightingShader.SetUniformBy(_gl, "light.ambient", ambientColor);
        LightingShader.SetUniformBy(_gl, "light.diffuse", diffuseColor); // darkened
        LightingShader.SetUniformBy(_gl, "light.specular", new Vector3(1.0f, 1.0f, 1.0f));
        LightingShader.SetUniformBy(_gl, "light.position", _lampPosition);

        //We're drawing with just vertices and no indices, and it takes 36 vertices to have a six-sided textured cube
        _gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)TexturedNormaledCube.Vertices.Length/ TexturedNormaledCube.VerticeSize);

        LampShader.UseBy(_gl);

        var lampMatrix = Matrix4x4.Identity
                         * Matrix4x4.CreateScale(0.2f)
                         * Matrix4x4.CreateTranslation(_lampPosition);
                         //* Matrix4x4.CreateTranslation(new Vector3(1.2f, 1.0f, 2.0f));

        LampShader.SetUniformBy(_gl, "uModel", lampMatrix);
        LampShader.SetUniformBy(_gl, "uView", _camera.GetViewMatrix());
        LampShader.SetUniformBy(_gl, "uProjection", _camera.GetProjectionMatrix());
        //We're drawing with just vertices and no indices, and it takes 36 vertices to have a six-sided textured cube
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    public void OnStop()
    {
        throw new NotImplementedException();
    }

    void ISystem.OnLoad()
    {
        throw new NotImplementedException();
    }

    public void OnUpdate(double dt)
    {

    }

    public void OnWindowFrameBufferResize(Vector2D<int> resize)
    {
        _gl.Viewport(resize);
    }

    private void OnDispose()
    {
        _logger.LogInformation("OpnGLContext Disposing...");
        _vbo.DisposeBy(_gl);
        _ebo.DisposeBy(_gl);
        VaoCube.DisposeBy(_gl);
        LampShader.DisposeBy(_gl);
        LightingShader.DisposeBy(_gl);
        DiffuseMap.DisposeBy(_gl);
        SpecularMap.DisposeBy(_gl);
    }

    private void Dispose(bool disposing)
    {
        if(!_disposedValue)
        {
            if (disposing)
            {
                OnDispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
        _logger.LogInformation("OpnGLContext Already Disposed...");
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
