using Godot;
using System;

public partial class TacticsCamera : CharacterBody3D
{
    public const float Speed = 5.0f;
    public const float ZoomSpeed = 1.0f;
    public const float RotateSpeed = 0.5f;
    public const float MinZoomDistance = 2.0f;
    public const float MaxZoomDistance = 10.0f;
    private Camera3D _camera;

    public override void _Ready()
    {
		_camera = GetNode<Camera3D>("Camera3D");
    }
    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
