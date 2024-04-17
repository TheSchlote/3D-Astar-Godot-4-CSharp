using Godot;
using System.Linq;

public partial class PlayerState : State
{
    private PlayerTurnUIPanel uiPanel;

    public override void Enter()
    {
        GD.Print("Player State");
        uiPanel = GetTree().Root.GetNode<PlayerTurnUIPanel>("BattleController/CanvasLayer/Panel");
        if (uiPanel != null)
        {
            uiPanel.Connect(nameof(PlayerTurnUIPanel.MoveRequest), new Callable(this, MethodName.OnMoveRequest));
        }
        var canvasLayer = GetTree().Root.GetNode<CanvasLayer>("BattleController/CanvasLayer");
        canvasLayer.Visible = true;
    }

    private void OnMoveRequest()
    {
        var battleController = GetTree().Root.GetNode<BattleController>("BattleController");
        Unit currentUnit = battleController.TurnOrder.Last();
        //currentUnit.MoveTo();
        GD.Print("Move requested by player. Select destination.");
    }

    public override void Exit()
    {
        uiPanel = GetTree().Root.GetNode<PlayerTurnUIPanel>("BattleController/CanvasLayer/Panel");
        if (uiPanel != null && uiPanel.IsConnected(nameof(PlayerTurnUIPanel.MoveRequest), new Callable(this, MethodName.OnMoveRequest)))
        {
            uiPanel.Disconnect(nameof(PlayerTurnUIPanel.MoveRequest), new Callable(this, MethodName.OnMoveRequest));
        }
        var canvasLayer = GetTree().Root.GetNode<CanvasLayer>("BattleController/CanvasLayer");
        canvasLayer.Visible = true;
    }
}
