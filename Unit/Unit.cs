using Godot;
using System;

public partial class Unit : Node3D
{
    [Export]
    public int Health { get; private set; } = 100;
    [Export]
    public int AttackValue { get; private set; } = 20;
    [Export]
    public int Defense { get; private set; } = 10;

    // Reference to the pathfinding and QTE system
    private SimpleAStarPathfinding pathfindingSystem;
    private QTEManager qteManager;

    // Additional properties to control the flow of the turn
    private bool hasMoved;
    private bool hasAttacked;

    // Events for the BattleSystem
    [Signal]
    public delegate void TurnEnded(Unit unit);

    public override void _Ready()
    {
        // Initialize your pathfinding and QTE system here
        // Assuming that the QTEManager and pathfinding system are siblings of this unit in the scene tree:
        qteManager = GetParent().GetNode<QTEManager>("QTEManager");
        pathfindingSystem = GetParent().GetNode<SimpleAStarPathfinding>("SimpleAStarPathfinding");
        hasMoved = false;
        hasAttacked = false;

        var uiPanel = GetNode<UIPanel>("/root/MainScene/UIPanel"); // Adjust the path to match your project structure
        uiPanel.Connect("ActionSelected", this, nameof(OnUIActionSelected));
    }

    private void OnUIActionSelected(string action)
    {
        if (action == "move")
        {
            // Handle move logic
        }
        else if (action == "attack")
        {
            // Handle attack logic
        }
        else if (action == "end_turn")
        {
            EndTurn();
        }
    }

    public void StartTurn()
    {
        GD.Print("Unit's turn started.");
        // Reset turn properties
        hasMoved = false;
        hasAttacked = false;
        // TODO: Show UI options for Move, Attack, End Turn, etc.
    }

    public void Move(Vector3 target)
    {
        if (hasMoved)
        {
            GD.Print("This unit has already moved this turn.");
            return;
        }
        // Call into the pathfinding system to move to the target
        // This would be more complex in practice and would involve pathfinding and unit movement logic.
        // For example:
        pathfindingSystem.UpdatePath((Vector3I)GlobalTransform.Origin, (Vector3I)target);
        // ... animate movement here ...
        hasMoved = true;
    }

    public void AttackTarget(Unit target)
    {
        if (hasAttacked)
        {
            GD.Print("This unit has already attacked this turn.");
            return;
        }
        // Begin the attack action, which will involve a QTE
        qteManager.StartQTE(QTEType.KeyPress); // You'll have defined the QTEType elsewhere
        // TODO: Connect to QTE completion signal to handle the result
        hasAttacked = true;
    }

    public void EndTurn()
    {
        GD.Print("Unit ended its turn.");
        EmitSignal(nameof(TurnEnded), this);
    }

    // This method would be called from UI controls, like buttons in a command menu.
    public void PerformAction(string action)
    {
        switch (action)
        {
            case "move":
                // TODO: Handle move logic, likely involving user input for destination selection
                break;
            case "attack":
                // TODO: Handle attack logic, likely involving selecting a target
                break;
            case "end_turn":
                EndTurn();
                break;
        }
    }

    // Methods to handle taking damage, dying, etc.
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle the unit's death
        GD.Print("Unit has died.");
        QueueFree(); // Remove the unit from the scene
    }
}
