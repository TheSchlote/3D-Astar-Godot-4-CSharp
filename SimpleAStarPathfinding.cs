using Godot;
using System;
using System.Collections.Generic;

public partial class SimpleAStarPathfinding : GridMap
{
    [Export]
    public Vector3I startPosition = new Vector3I(0, 0, 0);
    [Export]
    public Vector3I endPosition = new Vector3I(0, 0, 11);

    private AStar3D aStar = new AStar3D();
    private List<Vector3> path = new List<Vector3>(); // The path from start to end
    private Vector3I mapSize;

    private Vector3I minBounds;
    private Vector3I maxBounds;
    private Vector3I boundsOffset;

    private const string WalkableTileName = "WalkableTile";
    private const string WalkableHighlightedTileName = "WalkableHighlightedTile";

    public override void _Ready()
    {
        CalculateMapSize();
        InitializeAStar();
        FindPath();
        VisualDebugPath();
        HighlightPath();

        // After everything is set up, spawn capsules at the start and end positions
        SpawnCapsule(startPosition, Colors.Green); // Start position in green
        SpawnCapsule(endPosition, Colors.Red); // End position in red

    }

    private void SpawnCapsule(Vector3I gridPosition, Color color)
    {
        Vector3 worldPosition = ConvertGridToWorldPosition(gridPosition);
        var capsule = CreateCapsuleMeshInstance(color, worldPosition);
        AddChild(capsule);
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
        meshInstance.GlobalTransform = new Transform3D(Basis.Identity, worldPosition);
        return meshInstance;
    }
    private void CalculateMapSize()
    {
        minBounds = new Vector3I(int.MaxValue, int.MaxValue, int.MaxValue);
        maxBounds = new Vector3I(int.MinValue, int.MinValue, int.MinValue);

        foreach (Vector3I cell in GetUsedCells())
        {
            minBounds.X = Math.Min(minBounds.X, cell.X);
            minBounds.Y = Math.Min(minBounds.Y, cell.Y);
            minBounds.Z = Math.Min(minBounds.Z, cell.Z);

            maxBounds.X = Math.Max(maxBounds.X, cell.X);
            maxBounds.Y = Math.Max(maxBounds.Y, cell.Y);
            maxBounds.Z = Math.Max(maxBounds.Z, cell.Z);
        }

        mapSize = maxBounds - minBounds + new Vector3I(1, 1, 1);
        boundsOffset = -minBounds; // We'll use this to ensure all A* IDs are positive
    }
    private void InitializeAStar()
    {
        var addedCellIds = AddWalkableCellsToAStar();
        ConnectWalkableCells(addedCellIds);
        DebugAStarInitialization(addedCellIds); // Handles printing debug information
    }

    private HashSet<int> AddWalkableCellsToAStar()
    {
        HashSet<int> addedCellIds = new HashSet<int>();
        foreach (Vector3I cell in GetUsedCells())
        {
            if (IsWalkableCell(cell))
            {
                int cellId = AddCellToAStar(cell + boundsOffset); // Adjust cell position with offset
                addedCellIds.Add(cellId);
            }
        }
        return addedCellIds;
    }

    private void ConnectWalkableCells(HashSet<int> cellIds)
    {
        foreach (int cellId in cellIds)
        {
            Vector3I cellPosition = GetPositionFromCellId(cellId);
            foreach (var direction in new Vector3I[] { Vector3I.Right, Vector3I.Left, Vector3I.Forward, Vector3I.Back, Vector3I.Right + Vector3I.Up, Vector3I.Left + Vector3I.Up, Vector3I.Forward + Vector3I.Up, Vector3I.Back + Vector3I.Up, Vector3I.Right + Vector3I.Down, Vector3I.Left + Vector3I.Down, Vector3I.Forward + Vector3I.Down, Vector3I.Back + Vector3I.Down })
            {
                Vector3I neighborPosition = cellPosition + direction;
                if (cellIds.Contains(GetCellIdFromPosition(neighborPosition))) // Check if neighbor was added
                {
                    aStar.ConnectPoints(cellId, GetCellIdFromPosition(neighborPosition), true);
                }
            }
        }
    }

    private bool IsWalkableCell(Vector3I cell)
    {
        return GetCellItem(cell) == GetMeshLibraryItemIdByName(WalkableTileName);
    }

    private int AddCellToAStar(Vector3I cell)
    {
        int cellId = GetCellIdFromPosition(cell);
        aStar.AddPoint(cellId, new Vector3(cell.X, cell.Y, cell.Z), 1); // Assuming cell weights are uniform
        return cellId;
    }

    private void DebugAStarInitialization(HashSet<int> addedCellIds)
    {
        GD.Print("Added points count: ", addedCellIds.Count);
        bool startAdded = addedCellIds.Contains(GetCellIdFromPosition(startPosition + boundsOffset));
        bool endAdded = addedCellIds.Contains(GetCellIdFromPosition(endPosition + boundsOffset));
        GD.Print("Start Point Added: ", startAdded, ", End Point Added: ", endAdded);
    }
    private Vector3I GetPositionFromCellId(int cellId)
    {
        int z = cellId / (mapSize.X * mapSize.Y);
        cellId -= z * mapSize.X * mapSize.Y;
        int y = cellId / mapSize.X;
        int x = cellId % mapSize.X;
        return new Vector3I(x, y, z) - boundsOffset; // Adjust by boundsOffset to get original position
    }

    private bool IsCellInBounds(Vector3I cell)
    {
        // Check if the cell is within the bounds of the GridMap
        return cell.X >= 0 && cell.Y >= 0 && cell.Z >= 0 &&
               cell.X < mapSize.X && cell.Y < mapSize.Y && cell.Z < mapSize.Z;
    }

    private int GetCellIdFromPosition(Vector3I position)
    {
        // Ensure IDs are always positive
        return position.X + (position.Y * mapSize.X) + (position.Z * mapSize.X * mapSize.Y);
    }

    private void FindPath()
    {
        Vector3I offsetStart = startPosition + boundsOffset;
        Vector3I offsetEnd = endPosition + boundsOffset;

        int startId = GetCellIdFromPosition(offsetStart);
        int endId = GetCellIdFromPosition(offsetEnd);

        // Debug: Print the IDs for start and end positions
        GD.Print("Start ID: ", startId, ", End ID: ", endId);

        // Debug: Check if points exist before finding the path
        bool startPointExists = aStar.HasPoint(startId);
        bool endPointExists = aStar.HasPoint(endId);
        GD.Print("Start Point Exists: ", startPointExists, ", End Point Exists: ", endPointExists);

        if (!startPointExists || !endPointExists)
        {
            GD.PrintErr("Start or end point does not exist in AStar3D.");
            // Optionally, list all points to see if the IDs are as expected
            PrintAllAStarPoints();
            return;
        }

        Vector3[] pointPath = aStar.GetPointPath(startId, endId);

        if (pointPath == null || pointPath.Length == 0)
        {
            GD.PrintErr("No path found between start and end points.");
            PrintNeighborPoints(startId);
            PrintNeighborPoints(endId);
            return;
        }
        path.Clear();
        foreach (Vector3 cellPosition in pointPath) // Iterate over Vector3 positions
        {
            path.Add(cellPosition);
        }
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

    private void HighlightPath()
    {
        int highlightedTileId = GetMeshLibraryItemIdByName(WalkableHighlightedTileName);
        if (highlightedTileId == -1)
        {
            GD.PrintErr($"Highlighted tile '{WalkableHighlightedTileName}' not found in MeshLibrary.");
            return;
        }

        foreach (Vector3 worldPosition in path)
        {
            HighlightCellAtWorldPosition(worldPosition, highlightedTileId);
        }
    }

    private void HighlightCellAtWorldPosition(Vector3 worldPosition, int tileId)
    {
        Vector3I gridPosition = WorldToGrid(worldPosition);
        if (!IsCellInBounds(gridPosition))
        {
            GD.PrintErr($"Grid position out of bounds: {gridPosition}");
            return;
        }
        SetCellItem(gridPosition, tileId);
    }


    private Vector3I WorldToGrid(Vector3 worldPosition)
    {
        // Assuming CellSize.Y is the height of one layer of the grid.
        // Adjust this logic if your grid has multiple layers and you want to calculate the correct layer.
        int yGrid = (int)Mathf.Max(Mathf.Floor(worldPosition.Y / CellSize.Y), 0);
        Vector3I gridPosition = new Vector3I(
            (int)Math.Round((worldPosition.X - CellSize.X / 2) / CellSize.X),
            yGrid,
            (int)Math.Round((worldPosition.Z - CellSize.Z / 2) / CellSize.Z)
        );
        return gridPosition;
    }

    private void VisualDebugPath()
    {
        foreach (Vector3 worldPosition in path)
        {
            SpawnCapsule(WorldToGrid(worldPosition), Colors.Yellow); // Use a distinct color for debugging
        }
    }

    private void PrintAllAStarPoints()
    {
        foreach (var id in aStar.GetPointIds())
        {
            var point = aStar.GetPointPosition(id);
            GD.Print("Point ID: ", id, " Position: ", point);
        }
    }

    private void PrintNeighborPoints(int pointId)
    {
        GD.Print("Neighbors for point ID: ", pointId);
        foreach (var id in aStar.GetPointConnections(pointId))
        {
            var point = aStar.GetPointPosition(id);
            GD.Print("Neighbor ID: ", id, " Position: ", point);
        }
    }
}