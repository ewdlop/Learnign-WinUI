using System;

namespace SilkDotNetLibrary.OpenGL.Shader
{
    public interface IShader : IReadOnlyShader, IDisposable
    {
        void Use();
    }
}
