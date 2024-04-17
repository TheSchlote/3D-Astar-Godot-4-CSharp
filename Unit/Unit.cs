using Godot;
using System;

public partial class Unit : Node3D
{
    [Export]
    public Vector3I GridPosition { get; set; }
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
    public void MoveTo(Vector3I newGridPosition)
    {
        GridPosition = newGridPosition;
        SimpleAStarPathfinding battleArena = GetTree().Root.GetNode<SimpleAStarPathfinding>("BattleController/GridMap");
        Vector3 worldPosition = battleArena.MapToLocal(newGridPosition); // Ensure BattleArena is referenced correctly
        GlobalTransform = new Transform3D(GlobalTransform.Basis, worldPosition);
        GD.Print("Unit moved to: ", newGridPosition);
    }
}