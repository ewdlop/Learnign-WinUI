using System.Numerics;
using SharedLibrary.Event.EventArgs;

namespace SharedLibrary.Event.Listener;

public interface IMouseEventListener
{
    void OnMouseMove(object sender, (Vector2 Position, Vector2 LastMousePosition) e);
    void OnMouseWheel(object sender, (float x, float y) e);
}
