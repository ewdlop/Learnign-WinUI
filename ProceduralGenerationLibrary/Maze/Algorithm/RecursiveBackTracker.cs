namespace ProceduralGenerationLibrary.Maze.Algorithm;

public static class RecursiveBackTracker
{
    public static void On(this Grid grid)
    {
        Cell? startAt = grid.GetRandomCell();
        Stack<Cell?> stack = new();
        stack.Push(startAt);
        while(stack.Count > 0)
        {
            Cell? current = stack.Peek();
            List<Cell?> neighbors = (current?.Neighbors.Values ?? Enumerable.Empty<Cell>())
                .Where(neighborCell => neighborCell is not null && neighborCell._links.Count == 0).ToList();

            if (neighbors.Count == 0)
            {
                stack.Pop();
            }
            else
            {
                Cell? newNeighbor = neighbors[new Random().Next(0, neighbors.Count)];
                current?.Link(newNeighbor ?? throw new InvalidOperationException(), true);
                stack.Push(newNeighbor);
            }
        }
    }
}