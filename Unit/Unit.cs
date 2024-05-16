using Godot;
using System.Collections.Generic;

public partial class Unit : Node3D
{
    [Export]
    public Vector3I GridPosition { get; set; }
    [Export]
    public int MaxHealth { get; set; }
    [Export]
    public int CurrentHealth { get; set; }
    [Export]
    public int Attack {  get; set; }
    [Export]
    public int AttackRange { get; set; } = 1;
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
    public float MoveSpeed = 1f;

    private Queue<Vector3> pathQueue = new Queue<Vector3>();

    public int ActionPoints { get; set; } = 0;
    // Recalculate Action Points when speed is adjusted
    private void RecalculateActionPoints()
    {
        // This method can adjust AP according to new speed, such as scaling them proportionally
    }

    public void AccumulateActionPoints()
    {
        ActionPoints += Speed;
    }
    public void FollowPath(List<Vector3> path)
    {
        pathQueue.Clear();

        for (int i = 0; i <= Mathf.Min(path.Count, Movement); i++)
        {
            pathQueue.Enqueue(path[i]);
        }
        if (pathQueue.Count > 0)
        {
            SetProcess(true);
        }
    }
    public void AttackUnit(Unit target)
    {
        if (IsWithinAttackRange(target))
        {
            target.CurrentHealth -= Attack;
            GD.Print(UnitName + " attacks " + target.UnitName + " for " + Attack + " damage.");

            if (target.CurrentHealth <= 0)
            {
                GD.Print(target.UnitName + " has been defeated.");
            }
        }
        else
        {
            GD.Print("Target is out of range.");
        }
    }

    private bool IsWithinAttackRange(Unit target)
    {
        BattleArena pathfinder = GetTree().Root.GetNode<BattleArena>("BattleController/GridMap");

        int pathLength = pathfinder.GetPathBetweenUnits(GridPosition, target.GridPosition);

        return pathLength <= AttackRange && pathLength != int.MaxValue;
    }

    public override void _Process(double delta)
    {
        if (pathQueue.Count > 0)
        {
            Vector3 nextPos = pathQueue.Peek();
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
        GridPosition = (Vector3I)Position;
    }
}