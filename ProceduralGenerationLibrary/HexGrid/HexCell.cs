using System.Numerics;

namespace ProceduralGenerationLibrary.HexGrid;

public class HexCell
{
    public (int q, int r) AxialCoordinate;
    public Vector3 Position { get; set; }
    public HexCell(in int q, in int r, in float x, in float y)
    {
        AxialCoordinate = (q, r);
        Position = new Vector3(x, y, y);
    }
}