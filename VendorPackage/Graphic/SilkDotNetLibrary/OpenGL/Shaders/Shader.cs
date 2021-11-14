using Silk.NET.OpenGL;
using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace SilkDotNetLibrary.OpenGL.Shaders;

public struct Shader : IShader
{
    private bool disposedValue;

    private readonly GL _gl;
    public uint ShaderProgramHandle { get;  private set; }
    public Shader(GL gl)
    {
        disposedValue = false;
        _gl = gl;
        ShaderProgramHandle = _gl.CreateProgram();
    }

    public void Load(string vertexPath, string fragmentPath)
    {
        //Load the individual shaders.
        uint vertex = LoadShader(ShaderType.VertexShader, vertexPath);
        uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);
        //Create the shader program.
        //Attach the individual shaders.
        _gl.AttachShader(ShaderProgramHandle, vertex);
        _gl.AttachShader(ShaderProgramHandle, fragment);
        _gl.LinkProgram(ShaderProgramHandle);
        //Check for linking errors.
        _gl.GetProgram(ShaderProgramHandle, GLEnum.LinkStatus, out var linkStatus);
        if (linkStatus == 0)
        {
            throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(ShaderProgramHandle)}");
        }
        //Detach and delete the shaders
        _gl.DetachShader(ShaderProgramHandle, vertex);
        _gl.DetachShader(ShaderProgramHandle, fragment);
        _gl.DeleteShader(vertex);
        _gl.DeleteShader(fragment);
    }

    public async Task LoadAsync(string vertexPath, string fragmentPath)
    {
        //Load the individual shaders.
        uint vertex = await LoadShaderAsync(ShaderType.VertexShader, vertexPath);
        uint fragment = await LoadShaderAsync(ShaderType.FragmentShader, fragmentPath);
        //Create the shader program.
        ShaderProgramHandle = _gl.CreateProgram();
        //Attach the individual shaders.
        _gl.AttachShader(ShaderProgramHandle, vertex);
        _gl.AttachShader(ShaderProgramHandle, fragment);
        _gl.LinkProgram(ShaderProgramHandle);
        //Check for linking errors.
        _gl.GetProgram(ShaderProgramHandle, GLEnum.LinkStatus, out var linkStatus);
        if (linkStatus == 0)
        {
            throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(ShaderProgramHandle)}");
        }
        //Detach and delete the shaders
        _gl.DetachShader(ShaderProgramHandle, vertex);
        _gl.DetachShader(ShaderProgramHandle, fragment);
        _gl.DeleteShader(vertex);
        _gl.DeleteShader(fragment);
    }

    public void Use()
    {
        _gl.UseProgram(ShaderProgramHandle);
    }

    public void SetUniform(string name, int value)
    {
        int unifromLocation = _gl.GetUniformLocation(ShaderProgramHandle, name);
        if (unifromLocation == -1) //If GetUniformLocation returns -1 the uniform is not found.
        {
            throw new Exception($"{name} uniform not found on shader {ShaderProgramHandle}.");
        }
        _gl.Uniform1(unifromLocation, value);
    }

    public unsafe void SetUniform(string name, Matrix4x4 value)
    {
        //A new overload has been created for setting a uniform so we can use the transform in our shader.
        int unifromLocation = _gl.GetUniformLocation(ShaderProgramHandle, name);
        if (unifromLocation == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }
        _gl.UniformMatrix4(unifromLocation, 1, false, (float*)&value);
    }

    public void SetUniform(string name, float value)
    {
        int unifromLocation = _gl.GetUniformLocation(ShaderProgramHandle, name);
        if (unifromLocation == -1)
        {
            throw new Exception($"{name} uniform not found on shader {ShaderProgramHandle}.");
        }
        _gl.Uniform1(unifromLocation, value);
    }

    private uint LoadShader(ShaderType type, string path)
    {
        string src = File.ReadAllText(path);
        uint handle = _gl.CreateShader(type);
        _gl.ShaderSource(handle, src);
        _gl.CompileShader(handle);
        string infoLog = _gl.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }

        return handle;
    }

    private async Task<uint> LoadShaderAsync(ShaderType type, string path)
    {
        string src = await File.ReadAllTextAsync(path);
        uint handle = _gl.CreateShader(type);
        _gl.ShaderSource(handle, src);
        _gl.CompileShader(handle);
        string infoLog = _gl.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }

        return handle;
    }
    private void OnDispose() => _gl.DeleteProgram(ShaderProgramHandle);

    private void Dispose(bool disposing)
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
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Shader()
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
