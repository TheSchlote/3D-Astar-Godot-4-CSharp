using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AstarPath : Node3D
{
	public List<Vector3> PathNodeList = new List<Vector3>();
	private GridMap GridMapWorld;
    Godot.Collections.Array NonWalkableCells = new Godot.Collections.Array();
    private AStar3D AStar3DPath = new AStar3D();
	private Vector3 PathStartPos = Vector3.Zero;
	private Vector3 PathEndPos = Vector3.Zero;
    private Vector3 MapSize = Vector3.Zero;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}
    private void GetGridMapBounds()
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var minZ = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;
        var maxZ = int.MinValue;

        // Iterate over all cells in the GridMap
        foreach (Vector3 cell in GridMapWorld.GetUsedCells())
        {
            minX = Mathf.Min(minX, (int)cell.X);
            minY = Mathf.Min(minY, (int)cell.Y);
            minZ = Mathf.Min(minZ, (int)cell.Z);
            maxX = Mathf.Max(maxX, (int)cell.X);
            maxY = Mathf.Max(maxY, (int)cell.Y);
            maxZ = Mathf.Max(maxZ, (int)cell.Z);

        }
        GD.Print("Min X: " + minX);
        GD.Print("Min Y: " + minY);
        GD.Print("Min Z: " + minZ);
        GD.Print("Max X: " + maxX);
        GD.Print("Max Y: " + maxY);
        GD.Print("Max Z: " + maxZ);
        // The size of the GridMap in cells
        Vector3 gridSize = new Vector3(maxX - minX + 1, maxY - minY + 1, maxZ - minZ + 1);
        GD.Print("Grid Size: ", gridSize);
        MapSize = gridSize;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
    public void SetGridMap(GridMap gridMap)
    {
        GridMapWorld = gridMap;                                     

        InitAstarPathFind();
    }
    private void InitAstarPathFind()
    {
        GetGridMapBounds();  // Get the GridMap boundaries


        // Replace this with your method for determining non-walkable cells in a GridMap
        NonWalkableCells = GetNonWalkableCells();

        var walkableCellsList = AddWalkableCells(NonWalkableCells);  // Get the walkable cells


            ConnectWalkableCells(walkableCellsList);  // Connect walkable cells for orthogonal movement only
        

        // Set the start and end position in 3D space
        SetPathStartPosition(Vector3.Zero);
        SetPathEndPosition(Vector3.Zero);
    }
    private void SetPathStartPosition(Vector3 value)
    {
        // Convert the 3D value to a cell position, assuming value is a world position
        // If value is already a cell position, you can skip this conversion
        Vector3 cellPosition = GridMapWorld.ToLocal(value);

        // Check if the cell is non-walkable or outside the map
        if (NonWalkableCells.Contains(cellPosition) || IsCellOutsideTheMap(cellPosition))
        {
            // Cannot use this cell as start position
            return;
        }

        // Set the start position to the provided Vector3 value
        PathStartPos = cellPosition;

        // If the path end position is valid, and it's not the same as the start position
        if (PathEndPos != Vector3.Zero && PathEndPos != PathStartPos)
        {
            // Calculate the A* path
            CalculateAstarPath();
        }
    }
    private void SetPathEndPosition(Vector3 value)
    {
        // Convert the 3D value to a cell position, assuming value is a world position
        Vector3 cellPosition = GridMapWorld.ToLocal(value);

        // Check if the cell is non-walkable or outside the map
        if (NonWalkableCells.Contains(cellPosition) || IsCellOutsideTheMap(cellPosition))
        {
            // Cannot use this cell as end position
            return;
        }

        // Set the end position to the provided Vector3 value
        PathEndPos = cellPosition;
    }

    private void CalculateAstarPath()
    {
        // Calculate the start cell position unique identifier
        var cellStartId = CalculateUniqueCellIdentifier(PathStartPos);
        // Calculate the end cell position unique identifier
        var cellEndId = CalculateUniqueCellIdentifier(PathEndPos);
        // Get the path between the start and the end cell, as a List
        PathNodeList = AStar3DPath.GetPointPath(cellStartId, cellEndId).ToList();
    }


    private void ConnectWalkableCells(Godot.Collections.Array walkableCells)
    {
        // Loop through all the walkable cells
        foreach (Vector3 cell in walkableCells)
        {
            // Get the unique cell identifier for the cell
            var cellId = CalculateUniqueCellIdentifier(cell);

            // Define directions for neighboring cells: front, back, left, right
            Vector3[] directions = {
            new Vector3(1, 0, 0),    // Right
            new Vector3(-1, 0, 0),   // Left
            new Vector3(0, 0, 1),    // Front
            new Vector3(0, 0, -1),   // Back
        };

            // Check neighboring cells in each direction
            foreach (Vector3 direction in directions)
            {
                // Calculate the neighboring cell position
                Vector3 neighborCellPos = cell + direction;

                // Check if the neighboring cell is walkable and not too high or too low
                if (IsCellWalkableAndWithinOneLevel(neighborCellPos, cell))
                {
                    // Get the unique identifier for the neighboring cell
                    var neighborCellId = CalculateUniqueCellIdentifier(neighborCellPos);

                    // Connect the current cell with the neighboring cell in the AStar path
                    AStar3DPath.ConnectPoints(cellId, neighborCellId, false);
                }
            }
        }
    }

    // Helper method to determine if a neighboring cell is walkable and within one level of height
    private bool IsCellWalkableAndWithinOneLevel(Vector3 neighborCellPos, Vector3 currentCellPos)
    {
        // If the neighboring cell is outside the bounds of the map, it's not walkable
        if (IsCellOutsideTheMap(neighborCellPos))
        {
            return false;
        }

        // Check the cell above the neighbor to ensure it's not occupied
        if (GridMapWorld.GetCellItem(new Vector3I((int)neighborCellPos.X, (int)(neighborCellPos.Y + 1), (int)neighborCellPos.Z)) != -1)
        {
            return false; // There is a block above the neighboring cell, so it's not walkable
        }

        // Check the neighbor cell itself to see if it's walkable
        bool isNeighborCellWalkable = GridMapWorld.GetCellItem(new Vector3I((int)neighborCellPos.X, (int)neighborCellPos.Y, (int)neighborCellPos.Z)) == -1;

        // Check the height difference
        bool isWithinOneLevel = Math.Abs(GridMapWorld.GetCellItem(new Vector3I((int)currentCellPos.X, (int)currentCellPos.Y, (int)currentCellPos.Z)) - GridMapWorld.GetCellItem(new Vector3I((int)neighborCellPos.X, (int)neighborCellPos.Y, (int)neighborCellPos.Z))) <= 1;

        // The cell is walkable if it's within one level of the current cell and the neighbor cell is not occupied
        return isNeighborCellWalkable && isWithinOneLevel;
    }
    private bool IsCellOutsideTheMap(Vector3 cell)
    {
        // Assume MapSize represents the size of your grid and MapOrigin represents the starting point (lowest corner)
        Vector3 MapOrigin = new Vector3(0, 0, 0); // Adjust according to your grid's configuration
        Vector3 MapMaxBounds = MapOrigin + MapSize;

        // Check if the cell is outside the bounds
        if (cell.X < MapOrigin.X || cell.Y < MapOrigin.Y || cell.Z < MapOrigin.Z ||
            cell.X >= MapMaxBounds.X || cell.Y >= MapMaxBounds.Y || cell.Z >= MapMaxBounds.Z)
        {
            return true; // The cell is outside the map
        }

        return false; // The cell is inside the map
    }

    private Godot.Collections.Array GetNonWalkableCells()
    {
        // Create an array to hold non-walkable cells
        Godot.Collections.Array nonWalkableCells = new Godot.Collections.Array();

        // Loop through all cells in the GridMap
        for (int z = 0; z < MapSize.Z; ++z)
        {
            for (int y = 0; y < MapSize.Y; ++y)
            {
                for (int x = 0; x < MapSize.X; ++x)
                {
                    // Check the cell above the current cell
                    Vector3I cellAbove = new Vector3I(x, y + 1, z);

                    // If there is a cube in the cell above, the current cell is non-walkable
                    if (GridMapWorld.GetCellItem(cellAbove) != -1)
                    {
                        // Add the current cell to the non-walkable array
                        nonWalkableCells.Add(new Vector3(x, y, z));
                    }
                }
            }
        }

        return nonWalkableCells;
    }
    private Godot.Collections.Array AddWalkableCells(Godot.Collections.Array nonWalkableCells)
    {
        // Create a new array for the walkable cells
        Godot.Collections.Array walkableCells = new Godot.Collections.Array();

        // Loop through all cells on the GridMap
        for (int z = 0; z < MapSize.Z; ++z)
        {
            for (int y = 0; y < MapSize.Y; ++y)
            {
                for (int x = 0; x < MapSize.X; ++x)
                {
                    // Get the cell at the x, y, z position
                    var cell = new Vector3(x, y, z);

                    // If the cell is a non-walkable cell
                    if (nonWalkableCells.Contains(cell))
                    {
                        // Go to the next cell in the loop
                        continue;
                    }

                    // Add the cell to the walkable cells list
                    walkableCells.Add(cell);

                    // Calculate the unique cell id for the 3D position
                    var cellId = CalculateUniqueCellIdentifier(cell);

                    // Add the cell to the list of points in the AStar2D (which should be changed to an AStar for 3D)
                    AStar3DPath.AddPoint(cellId, cell);
                }
            }
        }

        // Return the list of walkable cells
        return walkableCells;
    }
    private int CalculateUniqueCellIdentifier(Vector3 cell)
    {
        // Assuming that MapSize is the size of the grid
        // and that no grid will be larger than some reasonable limits
        // (in this case, we're using 4096 as an arbitrary limit which you can adjust as needed)
        int uniqueId = (int)(cell.Z * MapSize.X * MapSize.Y + cell.Y * MapSize.X + cell.X);
        return uniqueId;
    }
    public bool SetAstarPath(Vector3 startPosition, Vector3 endPosition)
    {
        GD.Print("Path from: ", startPosition);
        GD.Print("Path to: ", endPosition);
        GD.Print("Path calculated: ", PathNodeList.Count);

        // Check if a GridMap has been provided to the A* search
        if (GridMapWorld == null)
        {
            // Write an error to the console about it
            GD.PrintErr("AstarPathFind - GridMap is null. Make sure to call SetGridMap(GridMap) on the node that is parent to the Astar Script.");
            return false;
        }

        // Convert the 3D start and end positions to grid coordinates
        Vector3 startCellPosition = GridMapWorld.ToLocal(startPosition);
        Vector3 endCellPosition = GridMapWorld.ToLocal(endPosition);

        // Update the path start and end positions to the cell positions
        this.PathStartPos = startCellPosition;
        this.PathEndPos = endCellPosition;

        // Optional: Check if the start and end positions are walkable. This requires your method to check if cells are walkable
        // For simplicity, this step is skipped here but can be added based on your game's logic

        // Calculate the A* path
        CalculateAstarPath();
        return true; // Assuming path calculation always succeeds, but you might add checks to ensure path exists
    }

    public void UpdatePath(Vector3 gridPosition)
    {
        GD.Print($"Updating path to grid position: {gridPosition}"); // Debug print

        var player = GetNode<Player>("/root/Main/Character");
        if (player != null)
        {
            Vector3 playerPosition = player.GlobalTransform.Origin;
            GD.Print($"Player's current position: {playerPosition}"); // Debug print

            SetPathStartPosition(playerPosition);
            SetPathEndPosition(gridPosition);

            CalculateAstarPath();
            HighlightPath();
        }
        else
        {
            GD.Print("Player node not found"); // Debug print
        }
    }
    public void HighlightPath()
    {
        GD.Print("Highlighting path..."); // Debug print
        // First, clear the previous highlights
        foreach (Vector3I cell in GridMapWorld.GetUsedCells())
        {
            Cube cubeInstance = GridMapWorld.GetNodeOrNull<Cube>("Cell_" + cell.ToString());
            cubeInstance?.SetAsPath(false);
        }

        // Now highlight the new path
        foreach (Vector3 node in PathNodeList)
        {
            Cube cubeInstance = GridMapWorld.GetNodeOrNull<Cube>("Cell_" + node.ToString());
            cubeInstance?.SetAsPath(true);
        }
    }
}