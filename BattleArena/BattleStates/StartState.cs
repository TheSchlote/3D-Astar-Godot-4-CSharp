using Godot;
using System;

public partial class StartState : State
{
    public override void Enter()
    {
        GD.Print("Start State");
        //Get the Players and Enemies
        var battleController = GetTree().Root.GetNode<BattleController>("BattleController");
        //spawn them
        for (int i = 0; i < battleController.EnemyUnits.Length; i++)
        {
            PackedScene unit = battleController.EnemyUnits[i];
            battleController.SpawnUnit(unit, new Vector3I(22, 0, 22), false); //TODO have multipe spawn locations
        }
        for (int i = 0; i < battleController.PlayerUnits.Length; i++)
        {
            PackedScene unit = battleController.PlayerUnits[i];
            battleController.SpawnUnit(unit, new Vector3I(2, 0, 2), true); //TODO have multipe spawn locations
        }
        //determine turn order
        battleController.DetermineTurnOrder();
        //switch to appropriate state
        battleController.NextTurn();
    }

    public override void Exit() 
    { 
        //any clean up thats needed
    }

    //Signals if needed
}
