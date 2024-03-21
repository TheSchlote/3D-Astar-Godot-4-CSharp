using Godot;
using System;

public partial class Unit : MeshInstance3D
{
    private GridMap WorldGridMap;       // The world tile map
    private AstarPath AstarPathFind;    // The AstarPath
                                        // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        WorldGridMap = GetParent().GetNode<GridMap>("GridMap");         // Get the TileMap node
        AstarPathFind = GetNode<AstarPath>("AstarPath");                // Get the AstarPathFind node
        AstarPathFind.SetGridMap(WorldGridMap);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public override void _Input(InputEvent @event)
    {
        // If a mouse button is pressed
        if (@event is InputEventMouseButton mbEvent && mbEvent.IsPressed())
        {
            // And it's the left one
            if (mbEvent.ButtonIndex == (int)ButtonList.Left)
            {
                Vector3 mouseIn3DPosition = GetMouseClickWorldPosition();  // You need to implement this method

                // If a path was found
                if (AstarPathFind.SetAstarPath(this.GlobalTransform.origin, mouseIn3DPosition))
                {
                    // Set path target as the first node in the found path
                    PlayerPathTarget = 1;
                }
            }
        }
    }

}
