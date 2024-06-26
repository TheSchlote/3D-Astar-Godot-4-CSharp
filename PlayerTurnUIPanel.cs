using Godot;

public partial class PlayerTurnUIPanel : Panel
{
    // Signal to emit when an action is selected
    [Signal]
    public delegate void ActionSelectedEventHandler(string action);
    [Signal]
    public delegate void MoveRequestEventHandler();

    public override void _Ready()
    {
        GetNode<Button>("MoveButton").Pressed += () => OnMoveButtonPressed();
        GetNode<Button>("AttackButton").Pressed += () => OnAttackButtonPressed();
        GetNode<Button>("EndTurnButton").Pressed += () => OnEndTurnButtonPressed();
    }

    private void OnMoveButtonPressed()
    {
        EmitSignal(nameof(MoveRequest));
    }

    private void OnAttackButtonPressed()
    {
        EmitSignal(nameof(ActionSelected), "attack");
    }

    private void OnEndTurnButtonPressed()
    {
        EmitSignal(nameof(ActionSelected), "end_turn");
    }
}