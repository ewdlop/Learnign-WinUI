namespace ProceduralGenerationLibrary.Maze;

public class Cell
{ 
    public int Row { get; }
    public int Column { get; }
    public bool IsVisited { get; private set; }
    public IReadOnlyDictionary<string, Cell?> Neighbors => _neighbors;
    private readonly Dictionary<string, Cell?> _neighbors;
    public readonly Dictionary<Cell, bool> _links;
    public Cell(int row, int column)
    {
        Row = row;
        Column = column;
        _neighbors = new Dictionary<string, Cell?>();
        _links = new Dictionary<Cell, bool>();
    }
    public void Link(Cell cell, bool bidirectional)
    {
        _links.Add(cell, true);
        if (bidirectional)
        {
            cell._links.Add(this, true);
        }
    }

    public void Unlink(Cell cell, bool bidirectional)
    {
        _links.Remove(cell);
        if (bidirectional)
        {
            cell._links.Remove(this);
        }
    }

    public bool IsLinked(Cell cell) => _links.ContainsKey(cell);

    public Distance Distances()
    {
        Distance distances = new(this);
        List<Cell> frontiercCells = new(){this};

        while (frontiercCells.Count > 0)
        {
            List<Cell> newFrontierCells = new();
            foreach(Cell cell in frontiercCells)
            {
                foreach (Cell linkedCell in cell._links.Keys.Where(key => !key.IsVisited))
                {
                    distances[linkedCell] = distances[cell] + 1;
                    linkedCell.IsVisited = true;
                    newFrontierCells.Add(linkedCell);
                }
            }
            frontiercCells = newFrontierCells;
        }
        distances[this] = 0;
        return distances;
    }

    public void AddNeighbor(string direction, Cell? cell) => _neighbors.Add(direction,cell);
}