using Godot;
using System.Collections.Generic;

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Won,
    Lost
}
public partial class BattleSystem : Node3D
{

    public BattleState State { get; private set; }

    private List<Unit> playerUnits;
    private List<Unit> enemyUnits;
    private int currentPlayerUnitIndex;

    public override void _Ready()
    {
        //playerUnits = new List<Unit>(GetTree().GetNodesInGroup("PlayerUnits"));
        //enemyUnits = new List<Unit>(GetTree().GetNodesInGroup("EnemyUnits"));
        State = BattleState.Start;
        StartTurn();
    }

    public void StartTurn()
    {
        // Determine who's turn it is, initiate actions accordingly
        if (State == BattleState.PlayerTurn)
        {
            currentPlayerUnitIndex = (currentPlayerUnitIndex + 1) % playerUnits.Count;
            Unit activeUnit = playerUnits[currentPlayerUnitIndex];
            activeUnit.StartTurn();
        }
        // You can expand this to include EnemyTurn logic, checks for end conditions, etc.
    }

    public void EndTurn()
    {
        if (AllEnemiesDefeated())
        {
            State = BattleState.Won;
            GD.Print("Won");
        }
        else if (AllPlayersDefeated())
        {
            State = BattleState.Lost;
            GD.Print("Lost");
        }
        else
        {
            // Go to the next turn
            State = State == BattleState.PlayerTurn ? BattleState.EnemyTurn : BattleState.PlayerTurn;
            StartTurn();
        }
    }

    private bool AllEnemiesDefeated()
    {
        // Logic to check if all enemy units have been defeated
        return true;
    }

    private bool AllPlayersDefeated()
    {
        // Logic to check if all player units have been defeated
        return true;
    }
}
