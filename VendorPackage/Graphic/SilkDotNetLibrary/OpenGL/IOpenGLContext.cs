using System.Threading.Tasks;
using SharedLibrary.Systems;
using SharedLibrary.渲染;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL;

public interface IOpenGLContext : IReadOnlyOpenGLContext, IRenderer, ISystem
{
    Task<GL> OnLoadAsync();
    void OnWindowFrameBufferResize(Vector2D<int> resize);
}
