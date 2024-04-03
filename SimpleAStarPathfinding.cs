using Godot;
using System;
using System.Collections.Generic;

public partial class SimpleAStarPathfinding : GridMap
{
    private AStar3D aStar = new AStar3D();
    private Vector3I startPosition = new Vector3I(2, 0, 2); // Set by your game logic
    private Vector3I endPosition = new Vector3I(1, 3, 0); // Set by your game logic
    private List<Vector3> path = new List<Vector3>(); // The path from start to end
    private Vector3I mapSize;

    private Vector3I minBounds;
    private Vector3I maxBounds;
    private Vector3I boundsOffset;
    public override void _Ready()
    {
        CalculateMapSize();
        InitializeAStar();
        FindPath();
        HighlightPath();
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
        // Holds the cell IDs that have been added to ensure we only connect existing points
        HashSet<int> addedCellIds = new HashSet<int>();

        foreach (Vector3I cell in GetUsedCells())
        {
            if (GetCellItem(cell) == 0) // Assuming 0 is the ID for walkable cells
            {
                Vector3I offsetCell = cell + boundsOffset; // Apply offset to ensure positive ID
                int cellId = GetCellIdFromPosition(offsetCell); // Use the offset cell for ID generation
                if (!addedCellIds.Contains(cellId))
                {
                    aStar.AddPoint(cellId, new Vector3(offsetCell.X, offsetCell.Y, offsetCell.Z), 1); // Convert to Vector3
                    addedCellIds.Add(cellId);
                }

                // Connect this cell to its neighbors
                foreach (var direction in new Vector3I[] { Vector3I.Right, Vector3I.Left, Vector3I.Forward, Vector3I.Back })
                {
                    Vector3I neighbor = cell + direction;
                    Vector3I offsetNeighbor = neighbor + boundsOffset; // Apply offset to neighbor
                    int neighborId = GetCellIdFromPosition(offsetNeighbor); // Use the offset neighbor for ID generation

                    // Ensure both the cell and its neighbor have been added before connecting
                    if (IsCellInBounds(offsetNeighbor) && GetCellItem(neighbor) == 0 && addedCellIds.Contains(neighborId))
                    {
                        aStar.ConnectPoints(cellId, neighborId, true); // Bi-directional connection
                    }
                }
            }
        }
        // Debug: Print how many points have been added
        GD.Print("Added points count: ", addedCellIds.Count);

        // Debug: Verify that the start and end points are added
        Vector3I offsetStart = startPosition + boundsOffset;
        Vector3I offsetEnd = endPosition + boundsOffset;
        int startId = GetCellIdFromPosition(offsetStart);
        int endId = GetCellIdFromPosition(offsetEnd);

        bool startAdded = addedCellIds.Contains(startId);
        bool endAdded = addedCellIds.Contains(endId);
        GD.Print("Start Point Added: ", startAdded, ", End Point Added: ", endAdded);
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
            return;
        }

        Vector3[] pointPath = aStar.GetPointPath(startId, endId);

        if (pointPath == null || pointPath.Length == 0)
        {
            GD.PrintErr("No path found between start and end points.");
            return;
        }
        path.Clear();
        foreach (Vector3 cellPosition in pointPath) // Iterate over Vector3 positions
        {
            path.Add(cellPosition);
        }
    }

    private void HighlightPath()
    {
        foreach (Vector3 cell in path)
        {
            // Highlight logic
            GD.Print("Path Cell: ", cell); // Placeholder action
        }
    }
}
