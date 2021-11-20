using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL;

public interface IOpenGLContext : IReadOnlyOpenGLContext
{
    unsafe GL OnLoad();
    unsafe void OnRender(double dt);
    void OnWindowFrameBufferResize(in Vector2D<int> resize);
    void OnUpdate(double dt);
}
