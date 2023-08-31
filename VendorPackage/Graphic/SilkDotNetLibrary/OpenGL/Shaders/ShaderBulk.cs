using Silk.NET.OpenGL;
using System;
using System.Buffers;

namespace SilkDotNetLibrary.OpenGL.Shaders;

public readonly struct ShaderBulk
{
    public readonly uint[] ShaderProgramHandles;
    public ShaderBulk(GL gl, int count)
    {
        ShaderProgramHandles = ArrayPool<uint>.Shared.Rent(count);
        for (int i = 0; i < count; i++)
        {
            ShaderProgramHandles[i] = gl.CreateProgram();
        }
    }

    public void LoadBy(GL gl, string[] vertex, string[] fragment)
    {
        for (int i = 0; i < ShaderProgramHandles.Length; i++)
        {
            uint shaderProgram = ShaderProgramHandles[i];
            uint vertexShader = LoadShader(gl, ShaderType.VertexShader, vertex[i]);
            uint fragmentShader = LoadShader(gl, ShaderType.FragmentShader, fragment[i]);
            gl.ShaderSource(vertexShader, vertex[i]);
            gl.ShaderSource(fragmentShader, fragment[i]);
            gl.CompileShader(vertexShader);
            gl.CompileShader(fragmentShader);
            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, fragmentShader);
            gl.LinkProgram(shaderProgram);
            gl.GetProgram(shaderProgram, GLEnum.LinkStatus, out var linkStatus);
            if (linkStatus == 0)
            {
                throw new Exception($"Program failed to link with error: {gl.GetProgramInfoLog(shaderProgram)}");
            }
            gl.DetachShader(shaderProgram, vertexShader);
            gl.DetachShader(shaderProgram, fragmentShader);
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);
        }
    }


    private static uint LoadShader(GL gl, ShaderType type, string text)
    {
        uint handle = gl.CreateShader(type);
        gl.ShaderSource(handle, text);
        gl.CompileShader(handle);
        string infoLog = gl.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }

        return handle;
    }

    //public void UseBy(GL gl)
    //{
    //    gl.UseProgram(ShaderProgramHandle);
    //}

    //public void SetUniformBy(GL gl, string name, int value)
    //{
    //    int uniformLocation = gl.GetUniformLocation(ShaderProgramHandle, name);
    //    if (uniformLocation == -1) //If GetUniformLocation returns -1 the uniform is not found.
    //    {
    //        throw new Exception($"{name} uniform not found on shader {ShaderProgramHandle}.");
    //    }
    //    gl.Uniform1(uniformLocation, value);
    //}

    //public unsafe void SetUniformBy(GL gl, string name, Matrix4x4 value)
    //{
    //    //A new overload has been created for setting a uniform so we can use the transform in our shader.
    //    int uniformLocation = gl.GetUniformLocation(ShaderProgramHandle, name);
    //    if (uniformLocation == -1)
    //    {
    //        throw new Exception($"{name} uniform not found on shader.");
    //    }
    //    gl.UniformMatrix4(uniformLocation, 1, false, (float*)&value);
    //}

    //public void SetUniformBy(GL gl, string name, float value)
    //{
    //    int uniformLocation = gl.GetUniformLocation(ShaderProgramHandle, name);
    //    if (uniformLocation == -1)
    //    {
    //        throw new Exception($"{name} uniform not found on shader {ShaderProgramHandle}.");
    //    }
    //    gl.Uniform1(uniformLocation, value);
    //}

    //public void SetUniformBy(GL gl, string name, Vector3 value)
    //{
    //    int location = gl.GetUniformLocation(ShaderProgramHandle, name);
    //    if (location == -1)
    //    {
    //        throw new Exception($"{name} uniform not found on shader.");
    //    }
    //    gl.Uniform3(location, value.X, value.Y, value.Z);
    //}

    //public void SetUniformBy(GL gl, string name, Vector4 value)
    //{
    //    int location = gl.GetUniformLocation(ShaderProgramHandle, name);
    //    if (location == -1)
    //    {
    //        throw new Exception($"{name} uniform not found on shader.");
    //    }
    //    gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
    //}


    //private void OnDispose(GL gl) => gl.DeleteProgram(ShaderProgramHandle);
    //public void DisposeBy(GL gl)
    //{
    //    OnDispose(gl);
    //}
}
