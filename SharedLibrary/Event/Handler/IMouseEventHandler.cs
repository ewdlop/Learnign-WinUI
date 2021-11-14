using SharedLibrary.Event.EventArgs;
using System;

namespace SharedLibrary.Event.Handler;

public interface IMouseEventHandler
{
    event EventHandler<MouseMoveEventArgs> OnMouseMove;
    event EventHandler<MouseScrollWheelEventArgs> OnMouseScrollWheel;
    void OnMouseMoveHandler(MouseMoveEventArgs e);
    void OnMouseScrollWheelHandler(MouseScrollWheelEventArgs e);
}
