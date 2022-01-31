namespace ProceduralGenerationLibrary.Maze.Algorithm;

public static class AldousBroder
{
    public static void On(this Grid grid)
    {
        Cell? cell = grid.GetRandomCell();
        int unvisited = grid.Size - 1;
        List<string> directions = new();
        while (unvisited > 0)
        {
            foreach (string direction in cell?.Neighbors.Keys?? Enumerable.Empty<string>())
            {
                directions.Add(direction);
            }
            int sample = new Random().Next(0, directions.Count);
            if (cell?.Neighbors[directions[sample]] is null) continue;
            Cell? neighbor = cell.Neighbors[directions[sample]];
            if (neighbor?._links.Count == 0)
            {
                cell.Link(neighbor, true);
                unvisited -= 1;
            }
            cell = neighbor;
        }
    }
}