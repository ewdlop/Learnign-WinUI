using System;
using System.Numerics;

namespace SharedLibrary.Event.Handler;

public interface IMouseEventHandler
{
    event EventHandler<(Vector2 Position, Vector2 LastMousePosition)> OnMouseMove;
    event EventHandler<(float x, float y)> OnMouseScrollWheel;
    void OnMouseMoveHandler((Vector2 Position, Vector2 LastMousePosition) e);
    void OnMouseScrollWheelHandler((float x, float y) e);
}
