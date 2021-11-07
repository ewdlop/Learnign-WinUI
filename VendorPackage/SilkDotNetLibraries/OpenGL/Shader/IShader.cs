using System;

namespace SilkDotNetLibraries.OpenGL.Shader
{
    public interface IShader : IReadOnlyShader, IDisposable
    {
        void Use();
    }
}
