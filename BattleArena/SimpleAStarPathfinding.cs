using Godot;
using System.Collections.Generic;

public partial class SimpleAStarPathfinding : GridMap
{
    public Vector3I StartPosition;
    public Vector3I EndPosition;

    private AStar3D aStar = new AStar3D();
    public List<Vector3> Path = new List<Vector3>();

    private const string WalkableTileName = "WalkableTile";
    private const string WalkableHighlightedTileName = "WalkableHighlightedTile";
    private const string NonWalkableTileName = "NonWalkableTile";
    public override void _Ready()
    {
        InitializeAStar();
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
        int startId = GetCellIdFromPosition(StartPosition);
        int endId = GetCellIdFromPosition(EndPosition);

        List<Vector3> fullPath = new List<Vector3>(aStar.GetPointPath(startId, endId));

        // Optionally, remove the start and end positions from the fullPath if they are not to be walkable
        Path.Clear();
        foreach (Vector3 pos in fullPath)
        {
            Vector3I gridPos = LocalToMap(pos - GlobalTransform.Origin);
            if (gridPos != StartPosition && gridPos != EndPosition)
            {
                Path.Add(pos);
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

    public void UpdatePath(Vector3I newStart, Vector3I newEnd)
    {
        StartPosition = newStart;
        EndPosition = newEnd;
        InitializeAStar();
        FindPath();
        HighlightPath();
    }

    private void HighlightPath()
    {
        int highlightedTileId = GetMeshLibraryItemIdByName(WalkableHighlightedTileName);
        foreach (Vector3 worldPosition in Path)
        {
            Vector3 localPosition = GlobalTransform.Origin + worldPosition;
            Vector3I gridPosition = LocalToMap(localPosition);
            SetCellItem(gridPosition, highlightedTileId);
        }
    }

    private void SpawnCapsules()
    {
        SpawnCapsule(StartPosition, Colors.Green);
        SpawnCapsule(EndPosition, Colors.Red);
    }

    private void SpawnCapsule(Vector3I gridPosition, Color color)
    {
        Vector3 localPosition = MapToLocal(gridPosition);
        Vector3 worldPosition = GlobalTransform.Origin + localPosition;
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