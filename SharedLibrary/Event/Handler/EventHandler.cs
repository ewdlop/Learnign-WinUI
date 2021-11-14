using SharedLibrary.Event.EventArgs;
using System;

namespace SharedLibrary.Event.Handler;

public class EventHandler : IEventHandler
{
    public event EventHandler<MouseMoveEventArgs> OnMouseMove;
    public event EventHandler<MouseScrollWheelEventArgs> OnMouseScrollWheel;
    public void OnMouseMoveHandler(MouseMoveEventArgs e)
    {
        OnMouseMove?.Invoke(this, e);
    }
    public void OnMouseScrollWheelHandler(MouseScrollWheelEventArgs e)
    {
        OnMouseScrollWheel?.Invoke(this, e);
    }
}
