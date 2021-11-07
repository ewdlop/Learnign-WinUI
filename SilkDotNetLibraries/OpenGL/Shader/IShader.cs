using System;

namespace _3rdPartyGraphicLibrary.OpenGL.Shader
{
    public interface IShader : IReadOnlyShader, IDisposable
    {
        void Use();
    }
}
