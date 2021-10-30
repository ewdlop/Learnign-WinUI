using Silk.NET.OpenGL;
using System;
using System.IO;
using System.Threading.Tasks;

namespace _3rdPartyGraphicLibrary.OpenGL.Shader
{
    public interface IShader
    {
        void Use();
    }
    public class Shader : IShader
    {
        private GL GL { get; set; }
        public uint ShaderProgramHandle { get; private set; }
        public Shader(GL gl)
        {
            GL = gl;
        }
        public async Task Load(string vertexPath, string fragmentPath)
        {
            //Load the individual shaders.
            uint vertex = await LoadShader(ShaderType.VertexShader, vertexPath);
            uint fragment = await LoadShader(ShaderType.FragmentShader, fragmentPath);
            //Create the shader program.
            ShaderProgramHandle = GL.CreateProgram();
            //Attach the individual shaders.
            GL.AttachShader(ShaderProgramHandle, vertex);
            GL.AttachShader(ShaderProgramHandle, fragment);
            GL.LinkProgram(ShaderProgramHandle);
            //Check for linking errors.
            GL.GetProgram(ShaderProgramHandle, GLEnum.LinkStatus, out var linkStatus);
            if (linkStatus == 0)
            {
                throw new Exception($"Program failed to link with error: {GL.GetProgramInfoLog(ShaderProgramHandle)}");
            }
            //Detach and delete the shaders
            GL.DetachShader(ShaderProgramHandle, vertex);
            GL.DetachShader(ShaderProgramHandle, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
        }

        public void Use()
        {
            GL.UseProgram(ShaderProgramHandle);
        }

        private async Task<uint> LoadShader(ShaderType type, string path)
        {
            //To load a single shader we need to:
            //1) Load the shader from a file.
            //2) Create the handle.
            //3) Upload the source to opengl.
            //4) Compile the shader.
            //5) Check for errors.
            string src = await File.ReadAllTextAsync(path);
            uint handle = GL.CreateShader(type);
            GL.ShaderSource(handle, src);
            GL.CompileShader(handle);
            string infoLog = GL.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
            }

            return handle;
        }
    }
}
