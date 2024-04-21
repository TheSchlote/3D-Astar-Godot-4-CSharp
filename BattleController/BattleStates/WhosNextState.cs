using Godot;
using System;
using System.Linq;

public partial class WhosNextState : State
{
    private BattleController battleController;
    public override void Enter()
    {
        GD.Print("WhosNext");
        battleController = GetTree().Root.GetNode<BattleController>("BattleController");
        battleController.DetermineTurnOrder(); // Ensure this method properly determines the next active unit

        if (battleController.TurnOrder.Count > 0)
        {
            Unit currentUnit = battleController.TurnOrder.Last();
            if (currentUnit.IsPlayerUnit)
            {
                battleStates.TransitionTo("Player");
            }
            else
            {
                battleStates.TransitionTo("Enemy");
            }
        }
    }

}
