using System.Threading.Tasks;
using SharedLibrary.Systems;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL;

public interface IOpenGLContext : IReadOnlyOpenGLContext, ISystem
{
    Task<GL> OnLoadAsync();
    void OnRender(double dt);
    void OnWindowFrameBufferResize(Vector2D<int> resize);
}
