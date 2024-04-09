using Godot;
using System.Collections.Generic;

public partial class SimpleAStarPathfinding : GridMap
{
    [Export]
    public Vector3I startPosition;
    [Export]
    public Vector3I endPosition;

    private AStar3D aStar = new AStar3D();
    private List<Vector3> path = new List<Vector3>();

    private const string WalkableTileName = "WalkableTile";
    private const string WalkableHighlightedTileName = "WalkableHighlightedTile";
    private const string NonWalkableTileName = "NonWalkableTile";

    public override void _Ready()
    {
        InitializeAStar();
    }

    // Call this method whenever you need to update the path, for example, when a unit moves.
    public void UpdatePath(Vector3I newStart, Vector3I newEnd)
    {
        // Make sure the start and end points exist in the AStar map before attempting to find a path
        if (!aStar.HasPoint(GetCellIdFromPosition(newStart)) || !aStar.HasPoint(GetCellIdFromPosition(newEnd)))
        {
            GD.PrintErr("AStar3D cannot find path because a point is missing:", newStart, newEnd);
            return;
        }
        InitializeAStar();
        startPosition = newStart;
        endPosition = newEnd;
        FindPath();
        HighlightPath();
    }

    public Vector3I FindWalkableCell()
    {
        foreach (Vector3I cell in GetUsedCells())
        {
            if (IsWalkableCell(cell))
            {
                return cell;
            }
        }
        GD.PrintErr("No walkable cell found.");
        return new Vector3I(); // Return an invalid position
    }

    public Unit SpawnUnit(PackedScene unitPrefab, Vector3I cellPosition)
    {
        Vector3 worldPosition = MapToLocal(cellPosition) + new Vector3(0, 1f, 0); // Adjust Y to half the unit's height if needed
        Unit unitInstance = unitPrefab.Instantiate<Unit>();
        unitInstance.GlobalTransform = new Transform3D(Basis.Identity, worldPosition);
        AddChild(unitInstance);
        SetCellItem(cellPosition, GetMeshLibraryItemIdByName(NonWalkableTileName));
        return unitInstance; // Make sure to return the instance
    }

    private void InitializeAStar()
    {
        foreach (Vector3I cell in GetUsedCells())
        {
            if (IsWalkableCell(cell))
            {
                AddCellToAStar(cell);
            }
        }
        ConnectWalkableCells();
    }

    private void AddCellToAStar(Vector3I cell)
    {
        int cellId = GetCellIdFromPosition(cell);
        Vector3 localPosition = MapToLocal(cell);
        Vector3 worldPosition = GlobalTransform.Origin + localPosition;
        aStar.AddPoint(cellId, worldPosition, 1);
        GD.Print("Added point with ID:", cellId, " at position ", worldPosition);
    }

    private bool IsWalkableCell(Vector3I cell)
    {
        return GetCellItem(cell) == GetMeshLibraryItemIdByName(WalkableTileName);
    }

    private void ConnectWalkableCells()
    {
        foreach (int cellId in aStar.GetPointIds())
        {
            ConnectCellNeighbors(cellId);
        }
    }

    private void ConnectCellNeighbors(int cellId)
    {
        Vector3I cellPosition = GetPositionFromCellId(cellId);
        Vector3I[] directions = {Vector3I.Right, Vector3I.Left, Vector3I.Forward, Vector3I.Back};

        foreach (Vector3I direction in directions)
        {
            Vector3I horizontalNeighbor = cellPosition + direction;
            Vector3I upperNeighbor = horizontalNeighbor + Vector3I.Up;
            Vector3I lowerNeighbor = horizontalNeighbor + Vector3I.Down;

            // Check for horizontal neighbor first
            if (IsWalkableCell(horizontalNeighbor))
            {
                ConnectIfPossible(cellPosition, horizontalNeighbor);
            }
            // Then check for upper diagonal neighbor
            else if (IsWalkableCell(upperNeighbor))
            {
                ConnectIfPossible(cellPosition, upperNeighbor);
            }
            // Finally, check for lower diagonal neighbor
            else if (IsWalkableCell(lowerNeighbor))
            {
                ConnectIfPossible(cellPosition, lowerNeighbor);
            }
        }
    }

    private void ConnectIfPossible(Vector3I from, Vector3I to)
    {
        int fromId = GetCellIdFromPosition(from);
        int toId = GetCellIdFromPosition(to);
        if (aStar.HasPoint(toId))
        {
            aStar.ConnectPoints(fromId, toId, true);
        }
    }

    private void FindPath()
    {
        if (!aStar.HasPoint(GetCellIdFromPosition(startPosition)) || !aStar.HasPoint(GetCellIdFromPosition(endPosition)))
        {
            GD.PrintErr("Start or end point does not exist in AStar map.");
            return;
        }
        int startId = GetCellIdFromPosition(startPosition);
        int endId = GetCellIdFromPosition(endPosition);

        List<Vector3> fullPath = new List<Vector3>(aStar.GetPointPath(startId, endId));

        path.Clear();
        foreach (Vector3 pos in fullPath)
        {
            Vector3I gridPos = LocalToMap(pos - GlobalTransform.Origin);
            if (gridPos != startPosition && gridPos != endPosition)
            {
                path.Add(pos);
            }
            else
            {
                int nonWalkableTileId = GetMeshLibraryItemIdByName(NonWalkableTileName);
                Vector3 localPosition = GlobalTransform.Origin + pos;
                Vector3I gridPosition = LocalToMap(localPosition);
                SetCellItem(gridPosition, nonWalkableTileId);
            }
        }
    }

    private void HighlightPath()
    {
        int highlightedTileId = GetMeshLibraryItemIdByName(WalkableHighlightedTileName);
        foreach (Vector3 worldPosition in path)
        {
            Vector3 localPosition = GlobalTransform.Origin + worldPosition;
            Vector3I gridPosition = LocalToMap(localPosition);
            SetCellItem(gridPosition, highlightedTileId);
        }
    }

    private int GetCellIdFromPosition(Vector3I position)
    {
        // Flat conversion assuming unique IDs across floors
        return position.X + position.Y * 1000 + position.Z * 1000000;
    }

    private Vector3I GetPositionFromCellId(int cellId)
    {
        int x = cellId % 1000;
        int y = (cellId / 1000) % 1000;
        int z = cellId / 1000000;
        return new Vector3I(x, y, z);
    }

    private int GetMeshLibraryItemIdByName(string name)
    {
        foreach (var key in MeshLibrary.GetItemList())
        {
            if (MeshLibrary.GetItemName(key) == name)
            {
                return key;
            }
        }
        GD.PrintErr($"MeshLibrary item with name '{name}' not found.");
        return -1; // Return an invalid ID if not found
    }
}