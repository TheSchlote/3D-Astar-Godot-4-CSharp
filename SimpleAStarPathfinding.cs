using Godot;
using System;
using System.Collections.Generic;

public partial class SimpleAStarPathfinding : GridMap
{
    [Export]
    public Vector3I startPosition;
    [Export]
    public Vector3I endPosition;

    private AStar3D aStar = new AStar3D();
    private List<Vector3> path = new List<Vector3>(); // The path from start to end

    private const string WalkableTileName = "WalkableTile";
    private const string WalkableHighlightedTileName = "WalkableHighlightedTile";
    public override void _Ready()
    {
        InitializeAStar();
        FindPath();
        HighlightPath();
        SpawnCapsules();
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
        Vector3 localPosition = MapToLocal(cell); // Convert grid position to local position
        Vector3 worldPosition = GlobalTransform.Origin + localPosition; // Convert local position to world position if needed
        aStar.AddPoint(cellId, worldPosition, 1); // Weight is 1 for uniform cost
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
        Vector3I[] directions = {
        Vector3I.Right, Vector3I.Left, Vector3I.Forward, Vector3I.Back, // horizontal directions
    };

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
        int startId = GetCellIdFromPosition(startPosition);
        int endId = GetCellIdFromPosition(endPosition);

        path = new List<Vector3>(aStar.GetPointPath(startId, endId));
    }

    private void HighlightPath()
    {
        int highlightedTileId = GetMeshLibraryItemIdByName(WalkableHighlightedTileName);
        foreach (Vector3 worldPosition in path)
        {
            Vector3 localPosition = GlobalTransform.Origin + worldPosition; // Convert world position to local position
            Vector3I gridPosition = LocalToMap(localPosition); // Convert local position to grid position
            SetCellItem(gridPosition, highlightedTileId);
        }
    }


    private void SpawnCapsules()
    {
        SpawnCapsule(startPosition, Colors.Green);
        SpawnCapsule(endPosition, Colors.Red);
    }

    private void SpawnCapsule(Vector3I gridPosition, Color color)
    {
        Vector3 localPosition = MapToLocal(gridPosition); // Convert grid position to local position
        Vector3 worldPosition = GlobalTransform.Origin + localPosition; // Convert local position to world position if needed
        var capsule = CreateCapsuleMeshInstance(color, worldPosition);
        AddChild(capsule);
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

    private Vector3 ConvertGridToWorldPosition(Vector3I gridPosition)
    {
        return MapToLocal(gridPosition) + new Vector3(CellSize.X / 2, CellSize.Y, CellSize.Z / 2);
    }
    private MeshInstance3D CreateCapsuleMeshInstance(Color color, Vector3 worldPosition)
    {
        MeshInstance3D meshInstance = new MeshInstance3D();
        CapsuleMesh capsuleMesh = new CapsuleMesh();
        meshInstance.Mesh = capsuleMesh;
        StandardMaterial3D matColor = new StandardMaterial3D() { AlbedoColor = color };
        meshInstance.SetSurfaceOverrideMaterial(0, matColor);
        worldPosition = worldPosition + new Vector3I(0, 1, 0);//adjust visual to stand on tile
        meshInstance.GlobalTransform = new Transform3D(Basis.Identity, worldPosition);
        return meshInstance;
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