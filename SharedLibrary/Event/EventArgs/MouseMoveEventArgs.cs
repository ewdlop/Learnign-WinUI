using System.Numerics;

namespace SharedLibrary.Event.EventArgs;

public class MouseMoveEventArgs : System.EventArgs
{
    public Vector2 Position { get; init; }
    public Vector2 LastMousePosition { get; init; }
}
