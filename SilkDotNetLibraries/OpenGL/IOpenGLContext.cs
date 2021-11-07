using System;

namespace SilkDotNetWrapper.OpenGL
{
    public interface IOpenGLContext : IDisposable, IReadOnlyOpenGLContext
    {
        unsafe void OnLoad();
        unsafe void OnRender(double dt);
        void OnUpdate(double dt);
    }
}
