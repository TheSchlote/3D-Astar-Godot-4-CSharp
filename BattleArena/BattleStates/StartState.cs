using Godot;
using System;

public partial class StartState : State
{
    public override void Enter()
    {
        //Get the Players and Enemies
        var battleController = GetTree().Root.GetNode<BattleController>("BattleController");
        //spawn them
        battleController.SpawnUnit(new Vector3I(3, 0, 3));
        //determine turn order
        //switch to appropriate state

        //For now lets just go to player state
        GD.Print("StartState");
        battleStates.TransitionTo("Player");
    }

    public override void Exit() 
    { 
        //any clean up thats needed
    }

    //Signals if needed
}
