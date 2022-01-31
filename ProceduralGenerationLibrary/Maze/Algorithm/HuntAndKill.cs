namespace ProceduralGenerationLibrary.Maze.Algorithm;

public static class HuntAndKill
{
    public static void On(this Grid grid)
    {
        Cell? current = grid.GetRandomCell();
        while (current is not null)
        {
            List<Cell?> unvisitedNeighbors = new();
            foreach (Cell? value in current.Neighbors.Values)
            {
                if(value is not null && value._links.Count == 0)
                {
                    unvisitedNeighbors.Add(value);
                }
            }
            if (unvisitedNeighbors.Count > 0)
            {
                Cell? neighbor = unvisitedNeighbors[new Random().Next(0, unvisitedNeighbors.Count)];
                current.Link(neighbor ?? throw new InvalidOperationException(), true);
                current = neighbor;
            }
            else
            {
                current = null;
            }
            for (int i = 0; i < grid._rows; i++)
            {
                for (int j = 0; j < grid._columns; j++)
                {
                    List<Cell?> visitedNeighbors = new List<Cell?>();
                    foreach (Cell? cell in grid[i,j]?.Neighbors.Values?? Enumerable.Empty<Cell>())
                    {
                        if (cell is null || cell._links.Count <= 0) continue;
                        visitedNeighbors.Add(cell);
                    }

                    if (grid[i, j]?._links.Count != 0 || visitedNeighbors.Count <= 0) continue;
                    current = grid[i,j];
                    Cell? neighbor = visitedNeighbors[new Random().Next(0, visitedNeighbors.Count)];
                    current?.Link(neighbor ?? throw new InvalidOperationException(), true);
                    break;
                }
            }
        }
    }
}