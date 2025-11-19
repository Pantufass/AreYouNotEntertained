using Godot;
using System;

public partial class EnemyCharacter : Character
{
    private static Player player = null;
    public override void _Ready()
    {
        player = GetNode<Player>("/root/Node2D/Player");
    }
    public override void _Process(double delta)
    {
        var direction = (player.GlobalPosition - GlobalPosition).Normalized();  
        Velocity = direction * Speed;
        MoveAndSlide();
    }   

    public void GetHit(int damage)
    {
        GD.Print($"Enemy hit for {damage} damage!");
        Health -= damage;
        if (Health <= 0)
        {
            QueueFree();
        }
    }
}
