using Godot;
using System;

public partial class Cube : MeshInstance3D
{
    [Export]
    public Material Material1;
    [Export]
    public Material Material2;

    public override void _Ready()
    {
        SetSurfaceOverrideMaterial(0, Material1);
    }
}
