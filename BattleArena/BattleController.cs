using Godot;
using System;
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
}
