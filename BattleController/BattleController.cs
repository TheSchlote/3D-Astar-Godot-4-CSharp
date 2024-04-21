using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class BattleController : Node3D
{
    [Export]
    public PackedScene[] PlayerUnits;
    [Export]
    public PackedScene[] EnemyUnits;
    [Export]
    public Node3D unitsContainer;
    [Export]
    public BattleStates BattleStates;
    [Export]
    public SimpleAStarPathfinding BattleArena;

    public List<Unit> TurnOrder = new List<Unit>();
    private const int ACTION_THRESHOLD = 100;


    public void SpawnUnit(PackedScene unitScene, Vector3I gridPosition, bool isPlayer)
    {
        
        Vector3 localPosition = BattleArena.MapToLocal(gridPosition);
        Vector3 worldPosition = GlobalTransform.Origin + localPosition;
        Unit unitInstance = unitScene.Instantiate<Unit>();
        if(isPlayer)
        {
            BattleArena.SetCellItem(gridPosition, BattleArena.GetMeshLibraryItemIdByName(BattleArena.PlayerOccupiedTileName));
        }
        else
        {
            BattleArena.SetCellItem(gridPosition, BattleArena.GetMeshLibraryItemIdByName(BattleArena.EnemyOccupiedTileName));
        }
        unitInstance.IsPlayerUnit = isPlayer;
        worldPosition = worldPosition + new Vector3I(0, 1, 0);//adjust visual to stand on tile
        unitInstance.Transform = new Transform3D(Basis.Identity, worldPosition);
        unitsContainer.CallDeferred("add_child", unitInstance);
        TurnOrder.Add(unitInstance);
    }
    public void DetermineTurnOrder()
    {
        // Units accumulate action points each tick
        foreach (var unit in TurnOrder)
        {
            unit.AccumulateActionPoints();
            GD.Print(unit.ActionPoints);
        }

        // Determine the next unit to act based on who reaches the action threshold first
        TurnOrder = TurnOrder.OrderBy(u => u.ActionPoints).ToList();
        if (TurnOrder.Any() && TurnOrder.Last().ActionPoints >= ACTION_THRESHOLD)
        {
            NextTurn();
        }
        GD.Print(TurnOrder);
    }
    public void NextTurn()
    {
        if (TurnOrder.Count == 0) return;

        Unit currentUnit = TurnOrder.Last(); // The unit with the highest AP acts next
        currentUnit.ActionPoints -= ACTION_THRESHOLD; // Reset AP after acting

        // Always transition to WhosNext regardless of the type of the unit
        BattleStates.TransitionTo("WhosNext");

        // Re-sort units based on remaining AP for subsequent actions
        TurnOrder.Sort((a, b) => b.ActionPoints.CompareTo(a.ActionPoints));
    }

}