using Godot;
using System;

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
        AstarPathFind = GetNode<AstarPath>("AstarPath");          // Get the AstarPathFind node
        AstarPathFind.SetGridMap(WorldGridMap);                   // Set the GridMap the A* should perform pathfinding on
    }

    public override void _Input(InputEvent @event)
    {
        // If a mouse button is pressed
        if (@event is InputEventMouseButton mbEvent && mbEvent.IsPressed())
        {
            // And it's the left one
            if (mbEvent.ButtonIndex == MouseButton.Left)
            {
                var camera = GetViewport().GetCamera3D();
                var mousePosition = mbEvent.Position;
                var from = camera.ProjectRayOrigin(mousePosition);
                var to = from + camera.ProjectRayNormal(mousePosition) * 1000;

                //// Cast a ray to detect grid position
                //var spaceState = GetWorld3D().DirectSpaceState;
                //var result = spaceState.IntersectRay(from, to);
                //if (result.Contains("collider") && result["collider"] is GridMap)
                //{
                //    var gridPosition = (Vector3)result["position"]; // This is a world position

                //    // If a path was found
                //    if (AstarPathFind.SetAstarPath(GlobalTransform.origin, gridPosition))
                //    {
                //        // Set path target as the first node in the found path
                //        PlayerPathTarget = 1;
                //    }
                //}
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        //WalkPath(delta);
    }

    //private void WalkPath(double delta)
    //{
    //    if (PlayerPathTarget < AstarPathFind.PathNodeList.Count)
    //    {
    //        // Get the target node to walk towards
    //        var targetNodeWorldPosition = AstarPathFind.PathNodeList[PlayerPathTarget];
    //        var direction = (targetNodeWorldPosition - GlobalTransform.origin).Normalized();
    //        velocity = direction * Speed;
    //        velocity.y += Gravity * delta; // Apply gravity

    //        // Move the Player towards the target node
    //        velocity = MoveAndSlide(velocity, Vector3.Up);

    //        // Check if close enough to target node to consider it "reached"
    //        if (GlobalTransform.origin.DistanceTo(targetNodeWorldPosition) < 1.0f)
    //        {
    //            PlayerPathTarget++; // Move to next node
    //        }
    //    }
    //    else
    //    {
    //        velocity = Vector3.Zero;
    //    }
    //}
}
