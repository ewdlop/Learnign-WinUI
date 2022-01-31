using ProceduralGenerationLibrary.Maze;

public static class Wilson
{
    public static void On(this Grid grid)
    {
        List<Cell?> unvisited = new();
        for (int i = 0; i < grid._rows; i++)
        {
            for (int j = 0; j < grid._columns; j++)
            {
                unvisited.Add(grid[i, j]);
            }
        }
        if(unvisited.Count <= 0) return;
        Cell? first = unvisited[new Random().Next(0, unvisited.Count)];
        unvisited.Remove(first);

        while (unvisited.Count > 0)
        {
            Cell? cell = unvisited[new Random().Next(0, unvisited.Count)];
            List<Cell?> path = new List<Cell?> { cell };
            while (unvisited.Contains(cell))
            {
                List<Cell?> random = new List<Cell?>();
                foreach (Cell? neighborCell in cell?.Neighbors.Values)
                {
                    if (neighborCell is null)
                    {
                        random.Add(neighborCell);
                    }
                }
                int sample = new Random().Next(0, random.Count);
                cell = random[sample];
                if (path.Contains(cell))
                {
                    var position = path.IndexOf(cell);
                    path = path.GetRange(0, position + 1);
                }
                else
                {
                    path.Add(cell);
                }
                for (int index = 0; index <= path.Count - 2; index++)
                {
                    path[index]?.Link(path[index + 1] ?? throw new InvalidOperationException(), true);
                    unvisited.Remove(path[index]);
                }
            }
        }
    }

}
