using SharedLibrary.Event.EventArgs;
using System;
using System.Numerics;

namespace SharedLibrary.Event.Handler;

public class EventHandler : IEventHandler
{
    public event EventHandler<(Vector2 Position, Vector2 LastMousePosition)> OnMouseMove;
    public event EventHandler<(float x, float y)> OnMouseScrollWheel;
    public event EventHandler<char> OnKeyBoardKeyDown;
    public event EventHandler<double> OnWindowUpdate;
    public void OnMouseMoveHandler((Vector2 Position, Vector2 LastMousePosition) e)
    {
        OnMouseMove?.Invoke(this, e);
    }
    public void OnMouseScrollWheelHandler((float x, float y) e)
    {
        OnMouseScrollWheel?.Invoke(this, e);
    }

    public void OnKeyBoardKeyDownHandler(char keyCode)
    {
        OnKeyBoardKeyDown?.Invoke(this, keyCode);
    }

    public void OnWindowUpdateUpdateHandler(double dt)
    {
        OnWindowUpdate?.Invoke(this,dt);
    }
}
