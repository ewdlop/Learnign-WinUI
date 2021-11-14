using SharedLibrary.Event.EventArgs;

namespace SharedLibrary.Event.Listener;

public interface IMouseEventListener
{
    void OnMouseMove(object sender, MouseMoveEventArgs e);
    void OnMouseWheel(object sender, MouseScrollWheelEventArgs e);
}
