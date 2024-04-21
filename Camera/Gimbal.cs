using Godot;

public partial class Gimbal : Node3D
{
    [Signal]
    public delegate void RequestPathUpdateEventHandler(Vector3I newEnd);

    public Camera3D Camera;
    public Node3D InnerGimbal;

    [Export] float maxZoom = 3.0f;
    [Export] float minZoom = 0.5f;
    [Export] float zoomSpeed = 0.08f;
    [Export] float speed = 0.3f;
    [Export] float dragSpeed = 0.005f;
    [Export] float acceleration = 0.08f;
    [Export] float mouseSensitivity = 0.005f;

    float zoom = 1.5f;
    Vector3 move;

    public override void _Ready()
    {
        InnerGimbal = GetNode<Node3D>("InnerGimbal");
        Camera = GetNode<Camera3D>("InnerGimbal/Camera3D");

        SimpleAStarPathfinding battleArena = GetTree().Root.GetNode<SimpleAStarPathfinding>("BattleController/GridMap");
        if (battleArena != null)
        {
            Connect(SignalName.RequestPathUpdate, new Callable(battleArena, "UpdatePath"));
        }
        else
        {
            GD.PrintErr("BattleArena node not found or incorrect path!");
        }
    }
    public override void _Input(InputEvent @event)
    {
        if (HandleRaycast(@event)) return;
        HandleCameraMovement(@event);
    }
    private bool CanChooseNewPath()
    {
        var battleController = GetTree().Root.GetNode<BattleController>("BattleController");
        battleController.BattleArena.ClearHighlightedPath();
        var currentState = battleController.BattleStates.CurrentState;
        if (currentState is PlayerState)
        {
            var playerState = currentState as PlayerState;
            if (playerState.hasMoved == false)
            {
                return true;
            }
            
        }
        return false;
    }
    private bool HandleRaycast(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
        {
            Vector2 mousePosition = GetViewport().GetMousePosition();
            Vector3 from = Camera.ProjectRayOrigin(mousePosition);
            Vector3 to = from + Camera.ProjectRayNormal(mousePosition) * 1000;
            PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
            PhysicsRayQueryParameters3D queryParameters = new PhysicsRayQueryParameters3D { From = from, To = to };

            var result = spaceState.IntersectRay(queryParameters);
            if (result.ContainsKey("collider") && result["collider"].Obj is GridMap gridMap)
            {
                Vector3 hitPosition = (Vector3)result["position"];
                Vector3 localHitPosition = gridMap.ToLocal(hitPosition);
                Vector3I gridPosition = GetStableGridPosition(localHitPosition, to - from);
                if (CanChooseNewPath())
                {

                    EmitSignal(nameof(RequestPathUpdate), gridPosition);
                }
                return true;
            }
        }
        return false;
    }
    private Vector3I GetStableGridPosition(Vector3 localHitPosition, Vector3 rayCastDirection)
    {
        int AdjustPosition(float position, float direction)
        {
            if (IsInteger(position))
            {
                return (int)Mathf.Floor(position + 0.5f * (direction < 0 ? -1 : 1));
            }
            else
            {
                return Mathf.RoundToInt(position - (0.5f));
            }
        }

        int x = AdjustPosition(localHitPosition.X, rayCastDirection.X);
        int y = IsInteger(localHitPosition.Y) ? (int)Mathf.Floor(localHitPosition.Y + 0.5f) - 1 : Mathf.RoundToInt(localHitPosition.Y - 0.5f);
        int z = AdjustPosition(localHitPosition.Z, rayCastDirection.Z);

        return new Vector3I(x, y, z);
    }

    private bool IsInteger(float value)
    {
        // Define a tolerance level, which might need tuning based on specific use cases or testing
        float tolerance = 0.0001f; // This tolerance can be adjusted as needed
        return Mathf.Abs(value - Mathf.Round(value)) < tolerance;
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