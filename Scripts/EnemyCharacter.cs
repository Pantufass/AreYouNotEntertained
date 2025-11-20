using Godot;
using System;

public partial class EnemyCharacter : Character
{
    private static Player player = null;

    public override void _Ready()
    {
        if (player == null)
        {
            var root = GetTree().CurrentScene ?? GetTree().Root;
            player = root.GetNodeOrNull<Player>("Player");
            if (player == null)
            {
                // fallback: search children
                foreach (var node in root.GetChildren())
                {
                    if (node is Player p)
                    {
                        player = p;
                        break;
                    }
                }
            }
        }

        var hit = GetNodeOrNull<Area2D>("HurtArea");
        if (hit != null)
        {
            hit.BodyEntered += OnHitBodyEntered;
        }
    }

    public void SetTemplateHealth(int hp)
    {
        Health = hp;
    }

    private void OnHitBodyEntered(Node body)
    {
        if (body is Player p)
        {
            p.TakeDamage(Attack);
        }
    }
    public override void _Process(double delta)
    {
        var direction = (player.GlobalPosition - GlobalPosition).Normalized();  
        Velocity = direction * Speed;
        MoveAndSlide();
    }   

    public void GetHit(float damage)
    {
        GD.Print($"Enemy hit for {damage} damage!");
        Health -= damage;
        if (Health <= 0)
        {
            QueueFree();
            Player.OnKillEnemy(Experience);
        }
    }
}
