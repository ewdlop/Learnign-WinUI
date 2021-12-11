using Silk.NET.OpenGL;
using System;
using System.IO;
using System.Numerics;

namespace SilkDotNetLibrary.OpenGL.Shaders;

public class Shader : IShader
{
    private bool disposedValue;
    public uint ShaderProgramHandle { get; }
    public string VertexPath { get; }
    public string FragmentPath { get; }
    public Shader(in GL gl, in string vertexPath, in string fragmentPath)
    {
        disposedValue = false;
        ShaderProgramHandle = gl.CreateProgram();
        VertexPath = vertexPath;
        FragmentPath = fragmentPath;
        LoadBy(gl);
    }

    public void LoadBy(in GL gl)
    {
        //Load the individual shaders.
        uint vertex = LoadShader(gl, ShaderType.VertexShader, VertexPath);
        uint fragment = LoadShader(gl, ShaderType.FragmentShader, FragmentPath);
        //Create the shader program.
        //Attach the individual shaders.
        gl.AttachShader(ShaderProgramHandle, vertex);
        gl.AttachShader(ShaderProgramHandle, fragment);
        gl.LinkProgram(ShaderProgramHandle);
        //Check for linking errors.
        gl.GetProgram(ShaderProgramHandle, GLEnum.LinkStatus, out var linkStatus);
        if (linkStatus == 0)
        {
            throw new Exception($"Program failed to link with error: {gl.GetProgramInfoLog(ShaderProgramHandle)}");
        }
        //Detach and delete the shaders
        gl.DetachShader(ShaderProgramHandle, vertex);
        gl.DetachShader(ShaderProgramHandle, fragment);
        gl.DeleteShader(vertex);
        gl.DeleteShader(fragment);
    }

    //public async Task LoadAsync(string vertexPath, string fragmentPath)
    //{
    //    //Load the individual shaders.
    //    uint vertex = await LoadShaderAsync(ShaderType.VertexShader, vertexPath);
    //    uint fragment = await LoadShaderAsync(ShaderType.FragmentShader, fragmentPath);
    //    //Attach the individual shaders.
    //    _gl.AttachShader(ShaderProgramHandle, vertex);
    //    _gl.AttachShader(ShaderProgramHandle, fragment);
    //    _gl.LinkProgram(ShaderProgramHandle);
    //    //Check for linking errors.
    //    _gl.GetProgram(ShaderProgramHandle, GLEnum.LinkStatus, out var linkStatus);
    //    if (linkStatus == 0)
    //    {
    //        throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(ShaderProgramHandle)}");
    //    }
    //    //Detach and delete the shaders
    //    _gl.DetachShader(ShaderProgramHandle, vertex);
    //    _gl.DetachShader(ShaderProgramHandle, fragment);
    //    _gl.DeleteShader(vertex);
    //    _gl.DeleteShader(fragment);
    //}

    public void UseBy(in GL gl)
    {
        gl.UseProgram(ShaderProgramHandle);
    }

    public void SetUniformBy(in GL gl, string name, int value)
    {
        int unifromLocation = gl.GetUniformLocation(ShaderProgramHandle, name);
        if (unifromLocation == -1) //If GetUniformLocation returns -1 the uniform is not found.
        {
            throw new Exception($"{name} uniform not found on shader {ShaderProgramHandle}.");
        }
        gl.Uniform1(unifromLocation, value);
    }

    public unsafe void SetUniformBy(in GL gl, string name, Matrix4x4 value)
    {
        //A new overload has been created for setting a uniform so we can use the transform in our shader.
        int unifromLocation = gl.GetUniformLocation(ShaderProgramHandle, name);
        if (unifromLocation == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }
        gl.UniformMatrix4(unifromLocation, 1, false, (float*)&value);
    }

    public void SetUniformBy(in GL gl, string name, float value)
    {
        int unifromLocation = gl.GetUniformLocation(ShaderProgramHandle, name);
        if (unifromLocation == -1)
        {
            throw new Exception($"{name} uniform not found on shader {ShaderProgramHandle}.");
        }
        gl.Uniform1(unifromLocation, value);
    }

    public void SetUniformBy(in GL gl, string name, Vector3 value)
    {
        int location = gl.GetUniformLocation(ShaderProgramHandle, name);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }
        gl.Uniform3(location, value.X, value.Y, value.Z);
    }

    private static uint LoadShader(in GL gl,ShaderType type, string path)
    {
        string src = File.ReadAllText(path);
        uint handle = gl.CreateShader(type);
        gl.ShaderSource(handle, src);
        gl.CompileShader(handle);
        string infoLog = gl.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }

        return handle;
    }

    //private async Task<uint> LoadShaderAsync(ShaderType type, string path)
    //{
    //    string src = await File.ReadAllTextAsync(path);
    //    uint handle = _gl.CreateShader(type);
    //    _gl.ShaderSource(handle, src);
    //    _gl.CompileShader(handle);
    //    string infoLog = _gl.GetShaderInfoLog(handle);
    //    if (!string.IsNullOrWhiteSpace(infoLog))
    //    {
    //        throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
    //    }

    //    return handle;
    //}
    private void OnDispose(in GL gl) => gl.DeleteProgram(ShaderProgramHandle);

    private void Dispose(bool disposing, in GL gl)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                OnDispose(gl);
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

    public void DisposeBy(in GL gl)
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true, gl);
        GC.SuppressFinalize(this);
    }
}
