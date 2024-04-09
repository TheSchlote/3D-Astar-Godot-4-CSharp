using Godot;
public partial class PlayerController : Node3D
{
    private GridCursor cursor;
    private BattleSystem battleSystem;

    public override void _Ready()
    {
        battleSystem = GetNode<BattleSystem>("/root/BattleArea");//GetNode<BattleSystem>("BattleArea"); // Adjust the path to your BattleSystem node
        cursor = GetParent().GetNode<GridCursor>("GridCursor"); // Adjust the path to your cursor node
    }

    public override void _Process(double delta)
    {
        var direction = new Vector3I();

        if (Input.IsActionJustPressed("ui_right"))
            direction += Vector3I.Right;
        if (Input.IsActionJustPressed("ui_left"))
            direction += Vector3I.Left;
        if (Input.IsActionJustPressed("ui_down"))
            direction += Vector3I.Back;
        if (Input.IsActionJustPressed("ui_up"))
            direction += Vector3I.Forward;

        cursor.Move(direction);

        if (Input.IsActionJustPressed("ui_select"))
        {
            GD.Print(cursor.CurrentPosition);
            Unit selectedUnit = battleSystem.GetActiveUnit();
            if (selectedUnit != null)
            {
                selectedUnit.Move(cursor.CurrentPosition);
            }
        }
    }
}