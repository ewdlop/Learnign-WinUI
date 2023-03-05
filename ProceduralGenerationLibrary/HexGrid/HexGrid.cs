namespace ProceduralGenerationLibrary.HexGrid;

public class HexGrid
{
    public required HexCell[][] HexCells { get; set; }
    public HexGrid(in int gridX, in int gridY, in float size)
    {
        HexCells = new HexCell[gridY][];
        int centerRow = (int)Math.Floor((double)gridY / 2);
        for (int r = 0; r < gridY; r++)
        {
            int rowSize = 2 * centerRow + 1 - Math.Abs(centerRow - r);
            HexCells[r] = new HexCell[rowSize];
            for (int q = 0; q < gridX; q++)
            {
                int column = q - Math.Max(0, centerRow - r);
                if (column >= 0 && column < rowSize)
                {
                    float x = (float)(size * (Math.Sqrt(3) * q + Math.Sqrt(3) / 2 * r));
                    float y = (float)(size * (3.0f / 2 * r));
                    HexCells[r][column] = new HexCell(q, r, x, y);
                }
            }
        }
    }
    public HexCell[] this[int i] => HexCells[i];
}