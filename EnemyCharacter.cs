using Godot;
using System;

public partial class EnemyCharacter : CharacterBody2D
{
    private static Player player = null;
    public override void _Ready()
    {
        player = GetNode<Player>("/root/Node2D/Player");
    }
    public override void _Process(double delta)
    {
        var direction = (player.GlobalPosition - GlobalPosition).Normalized();  
        Velocity = direction * 120;
        MoveAndSlide();
    }   
}
