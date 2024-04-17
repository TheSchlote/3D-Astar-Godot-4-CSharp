using Godot;
using System;
using System.Collections.Generic;

public partial class Unit : Node3D
{
    [Export]
    public Vector3 GridPosition { get; set; }
    [Export]
    public int MaxHealth { get; set; }
    [Export]
    public int CurrentHealth { get; set; }
    [Export]
    public int Movement { get; set; }
    [Export]
    public bool IsPlayerUnit { get; set; }
    [Export]
    public string UnitName { get; set; }

    private int _speed;
    [Export]
    public int Speed
    {
        get => _speed;
        set
        {
            _speed = value;
            RecalculateActionPoints();
        }
    }
    [Export]
    public float MoveSpeed = 1f; // Units per second

    private Queue<Vector3> pathQueue = new Queue<Vector3>();


    public int ActionPoints { get; set; } = 0;
    // Recalculate Action Points when speed is adjusted
    private void RecalculateActionPoints()
    {
        // This method can adjust AP according to new speed, such as scaling them proportionally
    }

    // Simulate the accumulation of action points
    public void AccumulateActionPoints()
    {
        ActionPoints += Speed;
    }

    public void FollowPath(List<Vector3> path)
    {
        pathQueue = new Queue<Vector3>(path);
        SetProcess(true); 
    }

    public override void _Process(double delta)
    {
        if (pathQueue.Count > 0)
        {
            Vector3 nextPos = pathQueue.Peek() + new Vector3(0,1,0);
            Vector3 direction = (nextPos - Position).Normalized();
            float step = (float)(MoveSpeed * delta);

            if ((Position - nextPos).Length() <= step)
            {
                nextPos = Position;
                Position = nextPos;
                pathQueue.Dequeue();

                if (pathQueue.Count == 0)
                {
                    SetProcess(false);  // Stop processing when the path is complete
                }
            }
            else
            {
                Position += direction * step;
            }
        }
    }
}