using System;

namespace SilkDotNetLibrary.OpenGL.Shaders;

public interface IShader : IReadOnlyShader, IDisposable
{
    void Use();
}
