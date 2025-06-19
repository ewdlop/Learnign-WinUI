using System;
using System.Numerics;

namespace SharedLibrary.Event.Handler;

public interface IWindowEventHandler
{
    event EventHandler<double> OnWindowUpdate;
    void OnWindowUpdateUpdateHandler(double dt);
    event EventHandler<Vector2> OnWindowFrameBufferResize;
    void OnWindowFrameBufferResizeHandler(Vector2 resize);
}