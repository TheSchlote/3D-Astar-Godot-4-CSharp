using Godot;
using System;
using System.Linq;
using static Godot.HttpRequest;

public partial class Gimbal : Node3D
{
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
        Position = new Vector3(Mathf.Clamp(Position.X, -20, 20), Position.Y, Mathf.Clamp(Position.Z, -20, 20));
    }

}
