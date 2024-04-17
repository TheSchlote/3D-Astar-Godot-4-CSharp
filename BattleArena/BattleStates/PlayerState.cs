using Godot;
using System.Linq;

public partial class PlayerState : State
{
    private PlayerTurnUIPanel uiPanel;
    private BattleController battleController;
    private Unit currentUnit;
    private CanvasLayer canvasLayer;
    public override void Enter()
    {
        GD.Print("Player State Entered");
        uiPanel = GetTree().Root.GetNode<PlayerTurnUIPanel>("BattleController/CanvasLayer/Panel");
        battleController = GetTree().Root.GetNode<BattleController>("BattleController");
        currentUnit = battleController.TurnOrder.Last();
        canvasLayer = GetTree().Root.GetNode<CanvasLayer>("BattleController/CanvasLayer");

        if (uiPanel != null)
        {
            uiPanel.Connect(nameof(PlayerTurnUIPanel.MoveRequest), new Callable(this, MethodName.OnMoveRequest));
        }
        canvasLayer.Visible = true;
    }
    private void OnMoveRequest()
    {
        GD.Print("Move requested by player. Waiting for destination selection.");
        // Here you should add code to allow the player to select a destination
        // For now, let's assume the destination is selected here for example purposes.
        var pathFinder = battleController.BattleArena;
        Vector3I destination = new Vector3I(10, 0, 10); // This should be set based on user input
        currentUnit.FollowPath(pathFinder.Path);
    }
    //private void OnMoveRequest()
    //{
    //    var battleController = GetTree().Root.GetNode<BattleController>("BattleController");
    //    Unit currentUnit = battleController.TurnOrder.Last();
    //    var pathFinder = battleController.BattleArena;
    //    foreach (Vector3 node in pathFinder.Path)
    //    {
    //        var aboveNode = new Vector3(node.X, node.Y + 1, node.Z);
    //        currentUnit.MoveTo(aboveNode);
    //    }
    //    //currentUnit.MoveTo();
    //    GD.Print("Move requested by player. Select destination.");
    //}

    public override void Exit()
    {
        uiPanel = GetTree().Root.GetNode<PlayerTurnUIPanel>("BattleController/CanvasLayer/Panel");
        canvasLayer = GetTree().Root.GetNode<CanvasLayer>("BattleController/CanvasLayer");

        if (uiPanel != null && uiPanel.IsConnected(nameof(PlayerTurnUIPanel.MoveRequest), new Callable(this, MethodName.OnMoveRequest)))
        {
            uiPanel.Disconnect(nameof(PlayerTurnUIPanel.MoveRequest), new Callable(this, MethodName.OnMoveRequest));
        }
        canvasLayer.Visible = false;
    }
}
