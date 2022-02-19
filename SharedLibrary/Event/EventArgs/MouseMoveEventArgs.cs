using System.Numerics;

namespace SharedLibrary.Event.EventArgs;

public record struct MouseMoveEventArgs
{
    public Vector2 Position { get; init; }
    public Vector2 LastMousePosition { get; init; }
}
