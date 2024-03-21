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

}
