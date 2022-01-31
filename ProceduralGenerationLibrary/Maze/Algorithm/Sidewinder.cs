namespace ProceduralGenerationLibrary.Maze.Algorithm;

public static class Sidewinder
{
    public static void On(this Grid grid)
    {
        for (int i = 0; i < grid._rows; i++)
        {
            List<Cell?> run = new();
            for (int j = 0; j < grid._columns; j++)
            {
                run.Add(grid[i, j]);
                if (grid[i, j]?.Neighbors["East"] is null 
                    ||grid[i, j]?.Neighbors["North"] is null && new Random().Next(0,2) == 0)
                {
                    Cell? member = run[new Random().Next(0, run.Count)];
                    if (member?.Neighbors["North"] is not null)
                    {
                        member.Link(member.Neighbors["North"] ?? throw new InvalidOperationException(), true);
                    }
                    run.Clear();
                }
                else
                {
                    grid[i, j]?.Link(grid[i, j]?.Neighbors["East"] ?? throw new InvalidOperationException(), true);
                }
            }
        }
    }
}