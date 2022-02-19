namespace SharedLibrary.Event.EventArgs;
public record struct KeyBoardKeyDownEventArgs
{
    public char KeyCode { get; init; }
}