using Godot;
using System;

public partial class UIPanel : Panel
{
    // Signal to emit when an action is selected
    [Signal]
    public delegate void ActionSelected(string action);

    public override void _Ready()
    {
        // Connect each button's pressed signal to a method
        GetNode<Button>("MoveButton").Connect("pressed", this, nameof(OnMoveButtonPressed));
        GetNode<Button>("AttackButton").Connect("pressed", this, nameof(OnAttackButtonPressed));
        GetNode<Button>("EndTurnButton").Connect("pressed", this, nameof(OnEndTurnButtonPressed));
    }

    private void OnMoveButtonPressed()
    {
        EmitSignal(nameof(ActionSelected), "move");
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
