using Godot;
using System;
using System.Collections.Generic;

public partial class AstarPath : Node3D
{
	public List<Vector3> PathNodeList = new List<Vector3>();
	private GridMap GridMapWorld;
	private AStar3D AStar3DPath = new AStar3D();
	private Vector3 PathStartPos = Vector3.Zero;
	private Vector3 PathEndPos = Vector3.Zero;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
