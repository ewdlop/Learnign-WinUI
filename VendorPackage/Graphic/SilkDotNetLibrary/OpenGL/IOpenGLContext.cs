namespace SilkDotNetLibrary.OpenGL;

public interface IOpenGLContext : IReadOnlyOpenGLContext
{
    unsafe void OnLoad();
    unsafe void OnRender(double dt);
    void OnUpdate(double dt);
}
