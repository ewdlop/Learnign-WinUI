namespace ProceduralGenerationLibrary.Maze.Algorithm;

public static class BinaryTree
{
    public static void On(this Grid grid)
    {
        for (int i = 0; i < grid._rows; i++)
        {
            for (int j = 0; j < grid._columns; j++)
            {
                List<Cell?> neighbors = new();
                if (grid[i, j]?.Neighbors["North"] is not null)
                {
                    neighbors.Add(grid[i, j]?.Neighbors["North"]);
                }
                if (grid[i, j]?.Neighbors["East"] is not null)
                {
                    neighbors.Add(grid[i, j]?.Neighbors["East"]);
                }
                int index = new Random().Next(0, neighbors.Count);
                if (neighbors.Count > 0)
                {
                    grid[i, j]?.Link(neighbors[index] ?? throw new InvalidOperationException(), true);
                }
            }
        }
    }

#if false
    public static Grid ToOn(this Grid grid)
    {
        Grid newGrid = new Grid(grid._rows, grid._columns, grid._floorPrefab, grid._wallPrefab, grid._cornerWallPrefab);

        for (int i = 0; i < grid._rows; i++)
        {
            for (int j = 0; j < grid._columns; j++)
            {
                List<Cell?> neighbors = new();
                if (grid[i, j]?.Neighbors["North"] is not null)
                {
                    neighbors.Add(grid[i, j]?.Neighbors["North"]);
                }
                if (grid[i, j]?.Neighbors["East"] is not null)
                {
                    neighbors.Add(grid[i, j]?.Neighbors["East"]);
                }
                int index = new Random().Next(0, neighbors.Count);
                if (neighbors.Count > 0)
                {
                    newGrid[i, j]?.Link(neighbors[index] ?? throw new InvalidOperationException(), true);
                }
            }
        }
        return newGrid;
    }
#endif
}