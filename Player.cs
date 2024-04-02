using Godot;
using System;
using System.Linq;

public partial class Player : CharacterBody3D
{
    private int PlayerPathTarget = 1;   // Set the Player path target to the 2nd node in the list
    private GridMap WorldGridMap;       // The world grid map
    private AstarPath AstarPathFind;    // The AstarPath adapted for 3D
    private Vector3 velocity = Vector3.Zero; // Player's velocity

    // Speed and gravity for the player movement
    public float Speed = 5.0f;
    public float Gravity = -9.8f;

    public override void _Ready()
    {
        WorldGridMap = GetParent().GetNode<GridMap>("GridMap");   // Get the GridMap node
        AstarPathFind = GetParent().GetNode<AstarPath>("AstarPath");          // Get the AstarPathFind node
        AstarPathFind.SetGridMap(WorldGridMap);                   // Set the GridMap the A* should perform pathfinding on
    }

    public override void _PhysicsProcess(double delta)
    {
        GD.Print($"Path count: {AstarPathFind.PathNodeList.Count}"); // Debug print
        if (PlayerPathTarget < AstarPathFind.PathNodeList.Count)
        {
            Vector3 targetPosition = AstarPathFind.PathNodeList[PlayerPathTarget];
            Vector3 direction = (targetPosition - GlobalTransform.Origin).Normalized();
            GlobalTransform = GlobalTransform.Translated(direction * Speed * (float)delta);

            if (GlobalTransform.Origin.DistanceTo(targetPosition) < 0.5f) // Adjust threshold as needed
            {
                PlayerPathTarget++; // Next target
                GD.Print($"Moving to next path target: {PlayerPathTarget}"); // Debug print
            }
        }
    }
}