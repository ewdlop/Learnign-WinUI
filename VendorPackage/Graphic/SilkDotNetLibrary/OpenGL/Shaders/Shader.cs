using Silk.NET.OpenGL;
using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace SilkDotNetLibrary.OpenGL.Shaders;

public readonly struct Shader : IShader
{
    public uint ShaderProgramHandle { get; }
    public Shader(GL gl)
    {
        ShaderProgramHandle = gl.CreateProgram();
    }

    public void LoadBy(GL gl,string vertexPath, string fragmentPath)
    {
        //Load the individual shaders.
        uint vertex = LoadShader(gl, ShaderType.VertexShader, vertexPath);
        uint fragment = LoadShader(gl, ShaderType.FragmentShader, fragmentPath);
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

    public async Task LoadAsync(GL gl,string vertexPath, string fragmentPath)
    {
        //Load the individual shaders.
        uint vertex = await LoadShaderAsync(gl,ShaderType.VertexShader, vertexPath);
        uint fragment = await LoadShaderAsync(gl,ShaderType.FragmentShader, fragmentPath);
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

    public void UseBy(GL gl)
    {
        gl.UseProgram(ShaderProgramHandle);
    }

    public void SetUniformBy(GL gl, string name, int value)
    {
        int uniformLocation = gl.GetUniformLocation(ShaderProgramHandle, name);
        if (uniformLocation == -1) //If GetUniformLocation returns -1 the uniform is not found.
        {
            throw new Exception($"{name} uniform not found on shader {ShaderProgramHandle}.");
        }
        gl.Uniform1(uniformLocation, value);
    }

    public unsafe void SetUniformBy(GL gl, string name, Matrix4x4 value)
    {
        //A new overload has been created for setting a uniform so we can use the transform in our shader.
        int uniformLocation = gl.GetUniformLocation(ShaderProgramHandle, name);
        if (uniformLocation == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }
        gl.UniformMatrix4(uniformLocation, 1, false, (float*)&value);
    }

    public void SetUniformBy(GL gl, string name, float value)
    {
        int uniformLocation = gl.GetUniformLocation(ShaderProgramHandle, name);
        if (uniformLocation == -1)
        {
            throw new Exception($"{name} uniform not found on shader {ShaderProgramHandle}.");
        }
        gl.Uniform1(uniformLocation, value);
    }

    public void SetUniformBy(GL gl, string name, Vector3 value)
    {
        int location = gl.GetUniformLocation(ShaderProgramHandle, name);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }
        gl.Uniform3(location, value.X, value.Y, value.Z);
    }

    public void SetUniformBy(GL gl, string name, Vector4 value)
    {
        int location = gl.GetUniformLocation(ShaderProgramHandle, name);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }
        gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
    }

    private static uint LoadShader(GL gl,ShaderType type, string path)
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

    private async Task<uint> LoadShaderAsync(GL gl, ShaderType type, string path)
    {
        string src = await File.ReadAllTextAsync(path);
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
    private void OnDispose(GL gl) => gl.DeleteProgram(ShaderProgramHandle);
    public void DisposeBy(GL gl)
    {
        OnDispose(gl);
    }
}
