using Godot;
using System.Collections.Generic;

public partial class SimpleAStarPathfinding : GridMap
{
    public Vector3I StartPosition;
    public Vector3I EndPosition;

    private AStar3D aStar = new AStar3D();
    public List<Vector3> Path = new List<Vector3>();

    public string WalkableTileName = "WalkableTile";
    private const string WalkableHighlightedTileName = "WalkableHighlightedTile";
    private const string NonWalkableTileName = "NonWalkableTile";
    public string PlayerOccupiedTileName = "PlayerOccupiedTile";
    public string EnemyOccupiedTileName = "EnemyOccupiedTile";

    private void InitializeAStar()
    {
        foreach (Vector3I cell in GetUsedCells())
        {
            if (IsWalkableCell(cell) || cell == StartPosition)
            {
                int cellId = GetCellIdFromPosition(cell);
                aStar.AddPoint(cellId, MapToLocal(cell), 1);
            }
        }
        foreach (int cellId in aStar.GetPointIds())
        {
            ConnectCellNeighbors(cellId);
        }
    }

    private bool IsWalkableCell(Vector3I cell)
    {
        return GetCellItem(cell) == GetMeshLibraryItemIdByName(WalkableTileName);
    }

    private void ConnectCellNeighbors(int cellId)
    {
        Vector3I cellPosition = GetPositionFromCellId(cellId);
        Vector3I[] directions = { Vector3I.Right, Vector3I.Left, Vector3I.Forward, Vector3I.Back };

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
        int startId = GetCellIdFromPosition(StartPosition);
        int endId = GetCellIdFromPosition(EndPosition);

        List<Vector3> fullPath = new List<Vector3>(aStar.GetPointPath(startId, endId));
        Path.Clear();
        foreach (Vector3 pos in fullPath)
        {
            Vector3I gridPos = LocalToMap(pos);
            if (!IsOccupiedCell(gridPos))
            {
                Path.Add(pos);
            }
        }
    }

    private bool IsOccupiedCell(Vector3I cell)
    {
        int item = GetCellItem(cell);
        return item == GetMeshLibraryItemIdByName(PlayerOccupiedTileName) || item == GetMeshLibraryItemIdByName(EnemyOccupiedTileName);
    }

    public void UpdatePath(Vector3I newEnd)
    {
        EndPosition = newEnd;
        aStar.Clear();
        InitializeAStar();
        FindPath();
        HighlightPath();
    }


    private void HighlightPath()
    {
        int highlightedTileId = GetMeshLibraryItemIdByName(WalkableHighlightedTileName);
        foreach (Vector3 worldPosition in Path)
        {
            Vector3I gridPosition = LocalToMap(worldPosition);
            SetCellItem(gridPosition, highlightedTileId);
        }
    }
    public void ClearHighlightedPath()
    {
        int normalTileId = GetMeshLibraryItemIdByName(WalkableTileName);
        foreach (Vector3I cell in GetUsedCells())
        {
            if (GetCellItem(cell) == GetMeshLibraryItemIdByName(WalkableHighlightedTileName))
            {
                SetCellItem(cell, normalTileId);
            }
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

    public int GetMeshLibraryItemIdByName(string name)
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