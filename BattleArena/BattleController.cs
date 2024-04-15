using Godot;
using System.Collections.Generic;
using System.Drawing;

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

    private List<Unit> _turnOrder;

    public void SpawnUnit(Vector3I gridPosition)
    {
        Vector3 localPosition = BattleArena.MapToLocal(gridPosition);
        Vector3 worldPosition = GlobalTransform.Origin + localPosition;
        Node3D unitInstance = PlayerUnits[0].Instantiate<Node3D>();//For now
        worldPosition = worldPosition + new Vector3I(0, 1, 0);//adjust visual to stand on tile
        unitInstance.Transform = new Transform3D(Basis.Identity, worldPosition);
        CallDeferred("add_child", unitInstance);
    }
}