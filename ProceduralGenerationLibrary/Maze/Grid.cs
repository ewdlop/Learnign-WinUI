namespace ProceduralGenerationLibrary.Maze;

public class Grid{

    public int _rows;
    public int _columns;
    protected readonly Cell?[,] _cells;
    public object _floorPrefab;
    public object _wallPrefab;
    public object _cornerWallPrefab;

    public Grid(int rows, int columns, object floorPrefab, object wallPrefab, object cornerWallPrefab)
    {
        _rows = rows;
        _columns = columns;
        _floorPrefab = floorPrefab;
        _wallPrefab = wallPrefab;
        _cornerWallPrefab = cornerWallPrefab;
        _cells = new Cell?[_rows, _columns];
        PrepareGrid();
        ConfigureCells();
    }

    public void PrepareGrid()
    {
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                this[i, j] = new Cell(i, j);
            }
        }
    }
    public void ConfigureCells()
    {
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                _cells[i, j]?.AddNeighbor("North", this[i + 1, j]);
                _cells[i, j]?.AddNeighbor("South", this[i - 1, j]);
                _cells[i, j]?.AddNeighbor("West", this[i, j - 1]);
                _cells[i, j]?.AddNeighbor("East", this[i, j + 1]);
            }
        }
    }
    
    public Cell? this[int rows, int columns] {
        get => rows >= 0
               && rows < _rows
               && columns >= 0
               && columns < _columns ? _cells[rows, columns] : null;
        set => _cells[rows, columns] = value;
    }
    public Cell? GetRandomCell() => _cells[new Random().Next(0, _rows), new Random().Next(0, _columns)]??null;

    public int Size => _rows * _columns;


#if false
    public Grid Copy()
    {
        Grid newGrid = new Grid(_rows, _columns, _floorPrefab, _wallPrefab, _cornerWallPrefab);
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                newGrid[i, j] = new Cell(i, j);
            }
        }
        return newGrid;
    }
#endif
}