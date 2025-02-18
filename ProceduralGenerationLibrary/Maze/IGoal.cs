namespace ProceduralGenerationLibrary.Maze;

public interface IGoal
{
    public bool IsPassageToGoal { get; }
    public bool IsPassageToFurthestGoal { get;}
    public bool IsGoal { get; }
    public bool IsExit { get; }
}
