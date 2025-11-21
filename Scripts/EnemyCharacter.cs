using Godot;
using System;

public partial class EnemyCharacter : Character
{
	private static Player player = null;
	private AnimatedSprite2D animation = null;
	private Color originalModulate = new Color(1,1,1,1);
	[Export]
	public double HitFlashDuration = 0.05;

	public override void _Ready()
	{
		// prefer the global Player.Instance if available (keeps references valid across scene reloads)
		player = Player.Instance ?? player;
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

		animation = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (animation != null)
		{
			originalModulate = animation.Modulate;
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
		if (GameState.IsGameOver)
			return;

		var currentPlayer = Player.Instance ?? player;
		if (currentPlayer == null)
			return;

		var direction = (currentPlayer.GlobalPosition - GlobalPosition).Normalized();
		Velocity = direction * Speed;
		MoveAndSlide();

		if (animation != null)
		{
			if (Velocity.X < 0)
				animation.FlipH = true;
			else if (Velocity.X > 0)
				animation.FlipH = false;
			if (!animation.IsPlaying())
				animation.Play("default");
		}
	}   

	public void GetHit(float damage)
	{
		GD.Print($"Enemy hit for {damage} damage!");
		Health -= damage;
		// flash red briefly to indicate hit
		if (animation != null)
		{
			animation.Modulate = new Color(1, 0.2f, 0.2f, 1);
			// reset after short duration
			GetTree().CreateTimer(HitFlashDuration).Timeout += () =>
			{
				if (animation != null)
					animation.Modulate = originalModulate;
			};
		}
		if (Health <= 0)
		{
			QueueFree();
			Player.OnKillEnemy(Experience);
		}
	}
}
