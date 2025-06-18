using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using SilkDotNetLibrary.OpenGL.Shaders;
using SilkDotNetLibrary.OpenGL.Meshes;
using SilkDotNetLibrary.OpenGL.Textures;
using SharedLibrary.Math;

namespace CoreLibrary;

public class AvocadoRenderer
{
    private readonly GL _gl;
    private SilkDotNetLibrary.OpenGL.Shaders.Shader _shader;
    private Mesh _mesh;
    private List<SilkDotNetLibrary.OpenGL.Textures.Texture> _textures;
    
    // Camera settings
    private Vector3 _cameraPosition = new Vector3(0, 0, 3.0f);
    private Vector3 _cameraTarget = Vector3.Zero;
    private Vector3 _cameraUp = Vector3.UnitY;
    
    // Model settings
    private Vector3 _modelPosition = Vector3.Zero;
    private Vector3 _modelScale = new Vector3(100.0f); // Scale up the tiny avocado!
    private Vector3 _modelRotation = Vector3.Zero;
    
    // Light settings
    private Vector3 _lightPosition = new Vector3(2.0f, 2.0f, 2.0f);
    private Vector3 _lightColor = Vector3.One;
    private Vector3 _ambientColor = new Vector3(0.2f);
    private Vector3 _diffuseColor = new Vector3(0.8f);
    private Vector3 _specularColor = Vector3.One;

    public AvocadoRenderer(GL gl)
    {
        _gl = gl;
    }

    public void Initialize()
    {
        Console.WriteLine("Initializing Avocado Renderer...");
        
        // Setup OpenGL state
        SetupOpenGLState();
        
        // Load shaders
        LoadShaders();
        
        // Load mesh and textures would be called from outside
        Console.WriteLine("Avocado Renderer initialized successfully!");
    }

    private void SetupOpenGLState()
    {
        // Enable depth testing
        _gl.Enable(EnableCap.DepthTest);
        _gl.DepthFunc(DepthFunction.Less);
        
        // Disable face culling initially for debugging
        _gl.Disable(EnableCap.CullFace);
        
        // Set clear color to dark gray for contrast
        _gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        
        Console.WriteLine("OpenGL state configured");
    }

    private void LoadShaders()
    {
        try 
        {
            _shader = new SilkDotNetLibrary.OpenGL.Shaders.Shader(_gl);
            
            // Load the debug shaders first to ensure visibility
            var vertexShader = System.IO.File.ReadAllText("Shaders/avocado_debug.vert");
            var fragmentShader = System.IO.File.ReadAllText("Shaders/avocado_debug.frag");
            
            _shader.LoadBy(_gl, vertexShader, fragmentShader);
            Console.WriteLine("Debug shaders loaded successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Shader loading failed: {ex.Message}");
            throw;
        }
    }

    public void SetMesh(Mesh mesh)
    {
        _mesh = mesh;
        Console.WriteLine("Mesh set");
    }

    public void SetTextures(List<SilkDotNetLibrary.OpenGL.Textures.Texture> textures)
    {
        _textures = textures;
        Console.WriteLine($"Textures set: {textures.Count} textures");
    }

    public void Render(int windowWidth, int windowHeight, float deltaTime)
    {
        if (_mesh.Equals(default) || _shader.Equals(default))
        {
            Console.WriteLine("Mesh or shader not initialized!");
            return;
        }

        // Clear the screen
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Calculate matrices
        var model = CalculateModelMatrix();
        var view = CalculateViewMatrix();
        var projection = CalculateProjectionMatrix(windowWidth, windowHeight);

        // Debug output every 60 frames (roughly once per second)
        if (Time.FrameCount % 60 == 0)
        {
            DebugMatrices(model, view, projection);
        }

        // Use shader and set uniforms
        _shader.UseBy(_gl);
        SetShaderUniforms(model, view, projection);

        // Draw the mesh
        if (_textures != null && _textures.Count > 0)
        {
            _mesh.Draw(_gl, _shader, _textures);
        }
        else
        {
            // Draw without textures for debug
            _mesh.Draw(_gl, _shader, new List<SilkDotNetLibrary.OpenGL.Textures.Texture>());
        }

        // Check for OpenGL errors
        CheckOpenGLErrors();
    }

    private Matrix4x4 CalculateModelMatrix()
    {
        var scale = Matrix4x4.CreateScale(_modelScale);
        var rotationX = Matrix4x4.CreateRotationX(_modelRotation.X);
        var rotationY = Matrix4x4.CreateRotationY(_modelRotation.Y);
        var rotationZ = Matrix4x4.CreateRotationZ(_modelRotation.Z);
        var translation = Matrix4x4.CreateTranslation(_modelPosition);
        
        return scale * rotationX * rotationY * rotationZ * translation;
    }

    private Matrix4x4 CalculateViewMatrix()
    {
        return Matrix4x4.CreateLookAt(_cameraPosition, _cameraTarget, _cameraUp);
    }

    private Matrix4x4 CalculateProjectionMatrix(int width, int height)
    {
        float aspectRatio = (float)width / height;
        return Matrix4x4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45.0f),
            aspectRatio,
            0.1f,
            100.0f);
    }

    private void SetShaderUniforms(Matrix4x4 model, Matrix4x4 view, Matrix4x4 projection)
    {
        try
        {
            // Set transformation matrices
            _shader.SetUniformBy(_gl, "uModel", model);
            _shader.SetUniformBy(_gl, "uView", view);
            _shader.SetUniformBy(_gl, "uProjection", projection);

            // Note: Debug shader doesn't need lighting uniforms
            // When you switch to the full shader, uncomment these:
            /*
            _shader.SetUniformBy(_gl, "light.position", _lightPosition);
            _shader.SetUniformBy(_gl, "light.ambient", _ambientColor);
            _shader.SetUniformBy(_gl, "light.diffuse", _diffuseColor);
            _shader.SetUniformBy(_gl, "light.specular", _specularColor);
            _shader.SetUniformBy(_gl, "viewPos", _cameraPosition);
            */
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting uniforms: {ex.Message}");
        }
    }

    private void DebugMatrices(Matrix4x4 model, Matrix4x4 view, Matrix4x4 projection)
    {
        Console.WriteLine("\n=== MATRIX DEBUG ===");
        Console.WriteLine($"Model scale: {_modelScale}");
        Console.WriteLine($"Camera position: {_cameraPosition}");
        Console.WriteLine($"Model determinant: {model.GetDeterminant():F6}");
        Console.WriteLine($"View determinant: {view.GetDeterminant():F6}");
        Console.WriteLine($"Projection determinant: {projection.GetDeterminant():F6}");
        
        // Check if matrices are reasonable
        if (Math.Abs(model.GetDeterminant()) < 0.0001f)
            Console.WriteLine("WARNING: Model matrix determinant near zero!");
        if (Math.Abs(view.GetDeterminant()) < 0.0001f)
            Console.WriteLine("WARNING: View matrix determinant near zero!");
        if (Math.Abs(projection.GetDeterminant()) < 0.0001f)
            Console.WriteLine("WARNING: Projection matrix determinant near zero!");
    }

    private void CheckOpenGLErrors()
    {
        var error = _gl.GetError();
        if (error != GLEnum.NoError)
        {
            Console.WriteLine($"OpenGL Error in AvocadoRenderer: {error}");
        }
    }

    // Public methods to control the avocado
    public void SetScale(float scale)
    {
        _modelScale = new Vector3(scale);
        Console.WriteLine($"Avocado scale set to: {scale}");
    }

    public void SetCameraPosition(Vector3 position)
    {
        _cameraPosition = position;
        Console.WriteLine($"Camera position set to: {position}");
    }

    public void SetCameraDistance(float distance)
    {
        _cameraPosition = new Vector3(0, 0, distance);
        Console.WriteLine($"Camera distance set to: {distance}");
    }

    public void RotateModel(Vector3 rotation)
    {
        _modelRotation += rotation;
    }

    // Switch to full shader when ready
    public void SwitchToFullShader()
    {
        try
        {
            var vertexShader = System.IO.File.ReadAllText("Shaders/avocado_simple.vert");
            var fragmentShader = System.IO.File.ReadAllText("Shaders/avocado_simple.frag");
            
            var newShader = new SilkDotNetLibrary.OpenGL.Shaders.Shader(_gl);
            newShader.LoadBy(_gl, vertexShader, fragmentShader);
            
            _shader = newShader;
            Console.WriteLine("Switched to full avocado shader!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to switch to full shader: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _shader.DisposeBy(_gl);
    }
}

// Helper class for frame counting
public static class Time
{
    public static int FrameCount { get; set; }
} 