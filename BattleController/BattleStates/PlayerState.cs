using Godot;
using System.Linq;

public partial class PlayerState : State
{
    private PlayerTurnUIPanel uiPanel;
    private BattleController battleController;
    private Unit currentUnit;
    private CanvasLayer canvasLayer;

    public bool hasMoved;
    private bool hasAttacked;
    public override void Enter()
    {
        GD.Print("Player State Entered");
        uiPanel = GetTree().Root.GetNode<PlayerTurnUIPanel>("BattleController/CanvasLayer/Panel");
        battleController = GetTree().Root.GetNode<BattleController>("BattleController");
        currentUnit = battleController.TurnOrder.Last();
        canvasLayer = GetTree().Root.GetNode<CanvasLayer>("BattleController/CanvasLayer");

        hasMoved = false;
        hasAttacked = false;

        if (uiPanel != null)
        {
            uiPanel.Connect(nameof(PlayerTurnUIPanel.MoveRequest), new Callable(this, MethodName.OnMoveRequest));
            uiPanel.Connect(nameof(PlayerTurnUIPanel.EndTurn), new Callable(this, MethodName.EndTurn));
            uiPanel.Connect(nameof(PlayerTurnUIPanel.Attack), new Callable(this, MethodName.OnAttackRequest));
        }
        battleController.BattleArena.StartPosition = (Vector3I)currentUnit.Position;

        canvasLayer.Visible = true;
    }
    private void OnMoveRequest()
    {
        if (!hasMoved)
        {
            GD.Print("Move requested by player. Waiting for destination selection.");
            var pathFinder = battleController.BattleArena;
            battleController.BattleArena.SetCellItem((Vector3I)currentUnit.Position, battleController.BattleArena.GetMeshLibraryItemIdByName(battleController.BattleArena.WalkableTileName));
            currentUnit.FollowPath(pathFinder.Path);
            battleController.BattleArena.ClearHighlightedPath();
            hasMoved = true;
        }
        else
        {
            GD.Print("Player has already moved this turn");
        }
    }
    private void OnAttackRequest()
    {
        if (!hasAttacked)
        {
            GD.Print("Attack initiated by player.");
            // Assume some attack functionality here.
            hasAttacked = true;
        }
        else
        {
            GD.Print("Player has already attacked this turn.");
        }
    }
    private void EndTurn()
    {
        Exit();
        battleStates.TransitionTo("WhosNext");
    }

    public override void Exit()
    {
        if (battleController is not null && battleController.BattleArena.Path.Count > 0)
        {
            battleController.BattleArena.Path.Clear();
            battleController.BattleArena.ClearHighlightedPath();
        }

        if (hasMoved)
        {
            battleController.BattleArena.SetCellItem((Vector3I)currentUnit.Position, battleController.BattleArena.GetMeshLibraryItemIdByName(battleController.BattleArena.PlayerOccupiedTileName));
        }

        uiPanel = GetTree().Root.GetNode<PlayerTurnUIPanel>("BattleController/CanvasLayer/Panel");
        canvasLayer = GetTree().Root.GetNode<CanvasLayer>("BattleController/CanvasLayer");

        if (uiPanel != null)
        {
            if (uiPanel.IsConnected(nameof(PlayerTurnUIPanel.MoveRequest), new Callable(this, MethodName.OnMoveRequest)))
            {
                uiPanel.Disconnect(nameof(PlayerTurnUIPanel.MoveRequest), new Callable(this, MethodName.OnMoveRequest));
            }

            if (uiPanel.IsConnected(nameof(PlayerTurnUIPanel.EndTurn), new Callable(this, MethodName.EndTurn)))
            {
                uiPanel.Disconnect(nameof(PlayerTurnUIPanel.EndTurn), new Callable(this, MethodName.EndTurn));
            }

            if (uiPanel.IsConnected(nameof(PlayerTurnUIPanel.Attack), new Callable(this, MethodName.OnAttackRequest)))
            {
                uiPanel.Disconnect(nameof(PlayerTurnUIPanel.Attack), new Callable(this, MethodName.OnAttackRequest));
            }
        }
        canvasLayer.Visible = false;
    }
}