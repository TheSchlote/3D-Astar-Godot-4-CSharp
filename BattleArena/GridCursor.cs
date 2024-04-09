using Godot;

public partial class GridCursor : Node3D
{
    [Export]
    public SimpleAStarPathfinding pathfindingSystem;
    public Vector3I CurrentPosition;
    public void SetPosition(Vector3I newPosition)
    {
        CurrentPosition = newPosition;
        UpdateVisualPosition();
    }

    public void Move(Vector3I direction)
    {
        SetPosition(CurrentPosition + direction);
    }

    private void UpdateVisualPosition()
    {
        Vector3 worldPosition = pathfindingSystem.MapToLocal(CurrentPosition);
        GlobalTransform = new Transform3D(Basis.Identity, worldPosition + new Vector3(0, 1f, 0));
    }
}
