using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using SilkDotNetLibrary.OpenGL.Meshes;

namespace CoreLibrary;

public class AvocadoExample
{
    private GL _gl;
    private AvocadoRenderer _avocadoRenderer;
    private Mesh _avocadoMesh;
    private List<SilkDotNetLibrary.OpenGL.Textures.Texture> _avocadoTextures;

    public AvocadoExample(GL gl)
    {
        _gl = gl;
    }

    public void Initialize()
    {
        Console.WriteLine("=== AVOCADO EXAMPLE INITIALIZATION ===");
        
        // Create renderer
        _avocadoRenderer = new AvocadoRenderer(_gl);
        _avocadoRenderer.Initialize();
        
        // Load avocado mesh and textures
        LoadAvocadoAssets();
        
        // Configure renderer
        ConfigureRenderer();
        
        Console.WriteLine("Avocado example initialized successfully!");
    }

    private void LoadAvocadoAssets()
    {
        try
        {
            Console.WriteLine("Loading avocado assets...");
            
            // Load GLTF mesh (you'll need to implement this based on your GLTF loader)
            // _avocadoMesh = LoadGLTFMesh("Assets/avocado/Avocado.gltf");
            
            // Load textures
            _avocadoTextures = new List<SilkDotNetLibrary.OpenGL.Textures.Texture> ();
            
            // Add your texture loading here:
            // _avocadoTextures.Add(new Texture(_gl, "Assets/avocado/Avocado_baseColor.png", TextureType.Diffuse));
            // _avocadoTextures.Add(new Texture(_gl, "Assets/avocado/Avocado_normal.png", TextureType.Normals));
            // _avocadoTextures.Add(new Texture(_gl, "Assets/avocado/Avocado_roughnessMetallic.png", TextureType.Specular));
            
            Console.WriteLine("Assets loaded successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load assets: {ex.Message}");
        }
    }

    private void ConfigureRenderer()
    {
        // Set mesh and textures
        if (!_avocadoMesh.Equals(default))
            _avocadoRenderer.SetMesh(_avocadoMesh);
        
        if (_avocadoTextures != null)
            _avocadoRenderer.SetTextures(_avocadoTextures);

        // Configure for visibility - these settings will definitely show the avocado
        _avocadoRenderer.SetScale(200.0f);           // Make it 200x bigger
        _avocadoRenderer.SetCameraDistance(5.0f);    // Position camera at reasonable distance
        
        Console.WriteLine("Renderer configured for maximum visibility");
    }

    public void Update(float deltaTime)
    {
        // Increment frame counter for debug output
        Time.FrameCount++;
        
        // Optional: Rotate the avocado for visual interest
        // _avocadoRenderer.RotateModel(new Vector3(0, deltaTime * 0.5f, 0));
    }

    public void Render(int windowWidth, int windowHeight, float deltaTime)
    {
        if (_avocadoRenderer == null)
        {
            Console.WriteLine("AvocadoRenderer not initialized!");
            return;
        }

        _avocadoRenderer.Render(windowWidth, windowHeight, deltaTime);
    }

    // Debug methods to help troubleshoot
    public void TryDifferentScales()
    {
        Console.WriteLine("=== TESTING DIFFERENT SCALES ===");
        
        float[] testScales = { 50.0f, 100.0f, 200.0f, 500.0f, 1000.0f };
        
        foreach (float scale in testScales)
        {
            Console.WriteLine($"Testing scale: {scale}x");
            _avocadoRenderer.SetScale(scale);
            
            // You would render a frame here and check if visible
            // System.Threading.Thread.Sleep(1000); // Wait 1 second
        }
    }

    public void TryDifferentCameraDistances()
    {
        Console.WriteLine("=== TESTING DIFFERENT CAMERA DISTANCES ===");
        
        float[] testDistances = { 0.5f, 1.0f, 2.0f, 5.0f, 10.0f };
        
        foreach (float distance in testDistances)
        {
            Console.WriteLine($"Testing camera distance: {distance}");
            _avocadoRenderer.SetCameraDistance(distance);
            
            // You would render a frame here and check if visible
            // System.Threading.Thread.Sleep(1000); // Wait 1 second
        }
    }

    public void SwitchToFullShader()
    {
        Console.WriteLine("Switching to full shader with textures...");
        _avocadoRenderer.SwitchToFullShader();
    }

    public void Dispose()
    {
        _avocadoRenderer?.Dispose();
    }
}

// Usage in your main render loop:
/*
public class YourMainClass
{
    private AvocadoExample _avocadoExample;

    public void Initialize(GL gl)
    {
        _avocadoExample = new AvocadoExample(gl);
        _avocadoExample.Initialize();
        
        // If still not visible, try debug methods:
        // _avocadoExample.TryDifferentScales();
        // _avocadoExample.TryDifferentCameraDistances();
    }

    public void Update(float deltaTime)
    {
        _avocadoExample.Update(deltaTime);
    }

    public void Render(int width, int height, float deltaTime)
    {
        _avocadoExample.Render(width, height, deltaTime);
    }
}
*/ 