using Godot;
using System;

public partial class EnemyState : State
{
    public override void Enter()
    {
        GD.Print("Enemy State");
        battleStates.TransitionTo("WhosNext");
    }
}