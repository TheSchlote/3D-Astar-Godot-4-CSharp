using Godot;
using System;

public partial class CapsuleSpawner : Node
{
	[Export]
	public NodePath GridMapPath;
	[Export]
	public PackedScene CapsuleScene;
	[Export]
	public Vector3I SpawnCellPosition = new Vector3I (0, 0, 0);
	private GridMap _gridMap;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(GridMapPath != null)
		{
			_gridMap = GetNode<GridMap>(GridMapPath);
		}
        if (_gridMap == null)
        {
            GD.Print("GridMap reference not set or path is incorrect.");
            return;
        }
        SpawnCapsuleAtCell(SpawnCellPosition);
    }

    private void SpawnCapsuleAtCell(Vector3I cellPosition)
    {
        if(_gridMap == null)
        {
            GD.Print("GridMap reference not set.");
            return;
        }

        if (CapsuleScene == null)
        {
            GD.Print("CapsuleScene not set.");
            return;
        }

        MeshInstance3D capsuleInstance = (MeshInstance3D)CapsuleScene.Instantiate();

        // Convert cell coordinates to world position
        Vector3 worldPosition = _gridMap.MapToLocal(cellPosition) + _gridMap.CellSize / 2; // Center in cell
        //capsuleInstance.Transform = new Transform(capsuleInstance.Transform.basis, worldPosition);


        // Add the capsule instance to the scene (as a child of this node for simplicity)
        AddChild(capsuleInstance);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
