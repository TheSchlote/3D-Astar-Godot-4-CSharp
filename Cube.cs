using Godot;

public partial class Cube : MeshInstance3D
{
    [Export]
    public Material DefaultMaterial;
    [Export]
    public Material HighlightMaterial;

    private bool isPath = false;

    public override void _Ready()
    {
        SetSurfaceOverrideMaterial(0, DefaultMaterial);
    }

    public void SetAsPath(bool isPath)
    {
        this.isPath = isPath;
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        SetSurfaceOverrideMaterial(0, isPath ? HighlightMaterial : DefaultMaterial);
    }
}
