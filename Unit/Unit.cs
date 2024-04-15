using Godot;
using System;

public partial class Unit : Node3D
{
    [Export]
    public Vector3I GridPosition { get; set; }
    [Export]
    public int Health { get; set; }
    [Export]
    public int Speed { get; set; }
}