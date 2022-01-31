using System.Diagnostics.CodeAnalysis;

namespace ProceduralGenerationLibrary.Maze;

public class Distance {
    
    public Cell _root;
    public readonly Dictionary<Cell, float> _cells;

    public Distance(Cell root)
    {
        _root = root;
        _cells = new Dictionary<Cell, float>
        {
            [_root] = 0
        };
    }

    public float this[Cell cell] {
        get => _cells[cell];
        set => _cells[cell] = value;
    }

    //public Distance PathTo(Cell goal, bool isPathToFurthestGoal)
    //{
    //    Cell current = goal;
    //    current.IsGoal = true;
    //    Distance breadcrumbs = new Distance(root);
    //    breadcrumbs[current] = cells[current];
    //    while (current != root)
    //    {
    //        foreach(DictionaryEntry neighbor in current.links)
    //        {
    //            if(cells[(Cell)(neighbor.Key)] < cells[current])
    //            {
    //                breadcrumbs[(Cell)(neighbor.Key)] = cells[(Cell)(neighbor.Key)];
    //                current = (Cell)(neighbor.Key);
    //                if (isPathToFurthestGoal)
    //                    current.IsPassageToFurthestGoal = true;
    //                else
    //                    current.IsPassageToGoal = true;

    //            }
    //        }
    //    }
    //    return breadcrumbs;
    //}
    public KeyValuePair<Cell, float> Max()
    {
        float maxDistance = 0;
        Cell maxCell = _root;
        foreach((Cell cell, float distance) in _cells)
        {
            if (!(distance > maxDistance)) continue;
            maxCell = cell;
            maxDistance = distance;
        }
        return new KeyValuePair<Cell, float>(maxCell, maxDistance);
    }
}