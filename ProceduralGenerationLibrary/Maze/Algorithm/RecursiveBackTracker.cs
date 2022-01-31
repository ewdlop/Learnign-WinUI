using ProceduralGenerationLibrary.Maze;

public static class RecursiveBackTracker
{
    public static void On(this Grid grid)
    {
        Cell? startAt = grid.GetRandomCell();
        Stack<Cell?> stack = new Stack<Cell?>();
        stack.Push(startAt);
        while(stack.Count > 0)
        {
            Cell? current = stack.Peek();
            List<Cell?> neighbors = new();

            foreach (Cell? neighborCell in current?.Neighbors.Values?? Enumerable.Empty<Cell>())
            {
                if (neighborCell is null || neighborCell._links.Count != 0) continue;
                neighbors.Add(neighborCell);
            }

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
