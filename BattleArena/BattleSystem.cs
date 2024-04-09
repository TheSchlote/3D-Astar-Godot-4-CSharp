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
    [Export]
    public BattleState State { get; private set; }
    [Export]
    public PackedScene unitPrefab;

    private List<Unit> playerUnits;
    private List<Unit> enemyUnits;
    private int currentPlayerUnitIndex;
    [Export]
    public SimpleAStarPathfinding pathfindingSystem;
    public override void _Ready()
    {
        if (pathfindingSystem == null)
        {
            GD.PrintErr("Pathfinding system is null");
        }
        else
        {
            SpawnPlayerUnits();
        }

        State = BattleState.Start;
        StartTurn();
    }

    private void SpawnPlayerUnits()
    {
        // Ensure the prefab is not null
        if (unitPrefab == null)
        {
            GD.PrintErr("Unit prefab is not assigned");
            return;
        }

        // Initialize the playerUnits list if not done already
        playerUnits = new List<Unit>();

        // Assume we want to spawn a single player unit for demonstration
        //Vector3I spawnCell = pathfindingSystem.FindWalkableCell();
        Unit unitInstance = pathfindingSystem.SpawnUnit(unitPrefab, new Vector3I(2, 0, 2));
        playerUnits.Add(unitInstance); // Add the new unit to the playerUnits list
    }
    public Unit GetActiveUnit()
    {
        if (playerUnits.Count == 0) return null; // Safety check
        return playerUnits[currentPlayerUnitIndex];
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
