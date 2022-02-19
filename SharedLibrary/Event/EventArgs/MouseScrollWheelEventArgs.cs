namespace SharedLibrary.Event.EventArgs;

public record struct MouseScrollWheelEventArgs
{
    public float X { get; init; }
    public float Y { get; init; }
}