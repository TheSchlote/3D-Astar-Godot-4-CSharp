using Godot;
using System.Collections.Generic;

public partial class BattleController : Node3D
{
    [Export]
    public PackedScene[] PlayerUnits;
    [Export]
    public PackedScene[] EnemyUnits;

    [Export]
    public BattleStates BattleStates;
    [Export]
    public SimpleAStarPathfinding BattleArena;

    public List<Unit> TurnOrder;

    public void SpawnUnit(PackedScene unit, Vector3I gridPosition)
    {
        Vector3 localPosition = BattleArena.MapToLocal(gridPosition);
        Vector3 worldPosition = GlobalTransform.Origin + localPosition;
        Node3D unitInstance = unit.Instantiate<Node3D>();
        worldPosition = worldPosition + new Vector3I(0, 1, 0);//adjust visual to stand on tile
        unitInstance.Transform = new Transform3D(Basis.Identity, worldPosition);
        unitInstance.GetNode("FX").GetNode<AnimationPlayer>("AnimationPlayer").Play("Swordandshieldidle");
        unitInstance.GetNode("FX").GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Swordandshieldidle").LoopMode = Animation.LoopModeEnum.Linear;
        CallDeferred("add_child", unitInstance);
    }
}