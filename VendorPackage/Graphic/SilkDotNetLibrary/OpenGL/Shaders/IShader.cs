using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL.Shaders;

public interface IShader: IReadOnlyShader
{
    void UseBy(in GL gl);
}
