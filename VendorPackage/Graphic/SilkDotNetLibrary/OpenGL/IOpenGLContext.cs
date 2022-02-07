using SharedLibrary.Systems;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL;

public interface IOpenGLContext : IReadOnlyOpenGLContext, ISystem
{
    GL OnLoad();
    void OnRender(double dt);
    void OnWindowFrameBufferResize(in Vector2D<int> resize);
}
