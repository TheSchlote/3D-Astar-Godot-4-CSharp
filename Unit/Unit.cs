using Godot;

public partial class Unit : Node3D
{
    [Export]
    public int Health { get; private set; } = 100;
    [Export]
    public int AttackValue { get; private set; } = 20;
    [Export]
    public int Defense { get; private set; } = 10;
    [Export]
    public int Movement { get; private set; } = 3;

    private SimpleAStarPathfinding pathfindingSystem;
    private QTEManager qteManager;

    private bool hasMoved;
    private bool hasAttacked;

    [Signal]
    public delegate void TurnEndedEventHandler();

    public override void _Ready()
    {
        qteManager = GetParent().GetParent().GetNode<QTEManager>("QTEManager");
        pathfindingSystem = GetParent().GetParent().GetNode<SimpleAStarPathfinding>("GridMap");
        hasMoved = false;
        hasAttacked = false;

        var uiPanel = GetNode<UIPanel>("/root/BattleArea/CanvasLayer/UIPanel");
        uiPanel.Connect(UIPanel.SignalName.ActionSelected, new Callable(this, MethodName.OnUIActionSelected));
    }

    private void OnUIActionSelected(string action)
    {
        switch (action)
        {
            case "move":
                GD.Print("Moving");
                
                break;
            case "attack":
                GD.Print("Attacking");
                break;
            case "end_turn":
                EndTurn();
                break;
            default:
                break;
        }
    }

    public void StartTurn()
    {
        GD.Print("Unit's turn started.");
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
