using Godot;

public partial class PlayerTurnUIPanel : Panel
{
    // Signal to emit when an action is selected
    [Signal]
    public delegate void AttackEventHandler(string action);
    [Signal]
    public delegate void MoveRequestEventHandler();
    [Signal]
    public delegate void EndTurnEventHandler();

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
        EmitSignal(nameof(Attack));
    }

    private void OnEndTurnButtonPressed()
    {
        EmitSignal(nameof(EndTurn));
    }
}