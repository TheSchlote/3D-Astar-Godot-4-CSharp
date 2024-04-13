using Godot;

public partial class Gimbal : Node3D
{
    [Export]
    public SimpleAStarPathfinding BattleArena;
    public Node3D InnerGimbal;
    public Camera3D Camera;

    [Export]
    float maxZoom = 3.0f;
    [Export]
    float minZoom = 0.5f;
    [Export]
    float zoomSpeed = 0.08f;

    float zoom = 1.5f;

    [Export]
    float speed = 0.3f;
    [Export]
    float dragSpeed = 0.005f;
    [Export]
    float acceleration = 0.08f;
    [Export]
    float mouseSensitivity = 0.005f;

    Vector3 move;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Camera = GetNode<Camera3D>("InnerGimbal/Camera3D");
        InnerGimbal = GetNode<Node3D>("InnerGimbal");
    }

    public override void _Input(InputEvent @event)
    {
        HandleCameraMovement(@event);
        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left && mouseButton.IsPressed())
        {
            Vector2 mousePosition = GetViewport().GetMousePosition();
            Vector3 from = Camera.ProjectRayOrigin(mousePosition);
            Vector3 to = from + Camera.ProjectRayNormal(mousePosition) * 1000; // Ray length of 1000 units
            PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
            PhysicsRayQueryParameters3D queryParameters = new PhysicsRayQueryParameters3D
            {
                From = from,
                To = to
            };
            Vector3 direction = (to - from).Normalized(); // Calculate the normalized direction of the ray

            GD.Print("Raycast direction: ", direction); // Print the direction of the raycast

            Godot.Collections.Dictionary result = spaceState.IntersectRay(queryParameters);

            if (result.ContainsKey("collider"))
            {
                var collider = result["collider"];
                if (collider.Obj is GridMap gridMap)
                {
                    Vector3 hitPosition = (Vector3)result["position"];
                    Vector3 localHitPosition = gridMap.ToLocal(hitPosition);
                    Vector3I gridPosition = GetStableGridPosition(localHitPosition, direction);

                    GD.Print("Hit Position: ", hitPosition, " Local Hit Position: ", localHitPosition, " Grid Position: ", gridPosition);
                    BattleArena.UpdatePath(BattleArena.startPosition, gridPosition);
                }
            }
        }
    }
    private Vector3I GetStableGridPosition(Vector3 localHitPosition, Vector3 rayCastDirection)
    {
        int x;
        if (IsInteger(localHitPosition.X) && rayCastDirection.X > 0 && rayCastDirection.Z > 0)
        {
            x = (int)localHitPosition.X;
        }
        else if (IsInteger(localHitPosition.X) && rayCastDirection.X < 0 && rayCastDirection.Z < 0)
        {
            x = (int)localHitPosition.X - 1;
        }
        else if (IsInteger(localHitPosition.X) && rayCastDirection.X < 0 && rayCastDirection.Z > 0)
        {
            x = (int)localHitPosition.X - 1;
        }
        else
        {
            x = Mathf.RoundToInt(localHitPosition.X - (BattleArena.CellScale / 2));
        }

        int y = IsInteger(localHitPosition.Y) ? (int)localHitPosition.Y - 1 : Mathf.RoundToInt(localHitPosition.Y - BattleArena.CellScale / 2);
        int z;
        if (IsInteger(localHitPosition.Z) && rayCastDirection.X > 0 && rayCastDirection.Z > 0)
        {
            z = (int)localHitPosition.Z;
        }
        else if (IsInteger(localHitPosition.Z) && rayCastDirection.X < 0 && rayCastDirection.Z < 0)
        {
            z = (int)localHitPosition.Z - 1;
        }
        else if (IsInteger(localHitPosition.Z) && rayCastDirection.X < 0 && rayCastDirection.Z > 0)
        {
            z = (int)localHitPosition.Z;
        }
        else if (IsInteger(localHitPosition.Z) && rayCastDirection.X > 0 && rayCastDirection.Z < 0)
        {
            z = (int)localHitPosition.Z - 1;
        }
        else
        {
            z = Mathf.RoundToInt(localHitPosition.Z - BattleArena.CellScale / 2);
        }

        return new Vector3I(x, y, z);
    }
    private bool IsInteger(float value)
    {
        return Mathf.Abs(value - Mathf.Round(value)) < Mathf.Epsilon;
    }
    private void HandleCameraMovement(InputEvent @event)
    {
        if (Input.IsActionPressed("rotate_cam") && @event is InputEventMouseMotion mouseMotion)
        {
            if (mouseMotion.Relative.X != 0)
            {
                RotateObjectLocal(Vector3.Up, -mouseMotion.Relative.X * mouseSensitivity);
            }

            if (mouseMotion.Relative.Y != 0)
            {
                float yRotation = Mathf.Clamp(-mouseMotion.Relative.Y, -30, 30);
                InnerGimbal.RotateObjectLocal(Vector3.Right, yRotation * mouseSensitivity);
            }
        }
        if (Input.IsActionPressed("move_cam") && @event is InputEventMouseMotion mouseDrag)
        {
            move.X -= mouseDrag.Relative.X * dragSpeed;
            move.Z -= mouseDrag.Relative.Y * dragSpeed;
        }

        if (@event.IsActionPressed("zoom_in"))
        {
            zoom -= zoomSpeed;
        }

        if (@event.IsActionPressed("zoom_out"))
        {
            zoom += zoomSpeed;
        }

        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    public static float Lerp(float First, float Second, float Amount)
    {
        return First * (1 - Amount) + Second * Amount;
    }
    public static Vector3 Lerp(Vector3 First, Vector3 Second, float Amount)
    {
        float retX = Lerp(First.X, Second.X, Amount);
        float retY = Lerp(First.Y, Second.Y, Amount);
        float retZ = Lerp(First.Z, Second.Z, Amount);
        return new Vector3(retX, retY, retZ);
    }
    public override void _Process(double delta)
    {
        Scale = Lerp(Scale, Vector3.One * zoom, zoomSpeed);
        InnerGimbal.Rotation = new Vector3(Mathf.Clamp(InnerGimbal.Rotation.X, -1.1f, 0.3f), InnerGimbal.Rotation.Y, InnerGimbal.Rotation.Z);
        MoveCam(delta);
    }

    private void MoveCam(double delta)
    {
        // Forward and backward movement
        if (Input.IsActionPressed("ui_down"))
        {
            move.Z = Lerp(move.Z, speed, acceleration); // Assuming positive Z is forward
        }
        else if (Input.IsActionPressed("ui_up"))
        {
            move.Z = Lerp(move.Z, -speed, acceleration); // Assuming negative Z is backward
        }
        else
        {
            move.Z = Lerp(move.Z, 0, acceleration); // Gradually stop moving along Z if no input
        }

        // Left and right movement
        if (Input.IsActionPressed("ui_left"))
        {
            move.X = Lerp(move.X, -speed, acceleration); // Move left
        }
        else if (Input.IsActionPressed("ui_right"))
        {
            move.X = Lerp(move.X, speed, acceleration); // Move right
        }
        else
        {
            move.X = Lerp(move.X, 0, acceleration); // Gradually stop moving along X if no input
        }

        // Apply movement
        Position += move.Rotated(Vector3.Up, Rotation.Y) * zoom;
        // Clamp position to prevent moving out of bounds
        Position = new Vector3(Mathf.Clamp(Position.X, 0, 25), Position.Y, Mathf.Clamp(Position.Z, 0, 25));
    }
}