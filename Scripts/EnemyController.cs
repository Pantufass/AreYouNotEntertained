using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyController : Node2D
{
	[Export]
	public PackedScene EnemyScene;

	[Export]
	public float MinSpawnDistance = 300.0f;
	[Export]
	public float MaxSpawnDistance = 600.0f;

	[Export]
	public double SpawnPeriod = 2.0;

	[Export]
	public Vector2 SpawnArea = new Vector2(1200, 600);

	private double timer = 0.0;
	private RandomNumberGenerator rng = new RandomNumberGenerator();
	private Player player = null;

	public override void _Ready()
	{
		rng.Randomize();
		var rect = GetViewport().GetVisibleRect();
		GlobalPosition = rect.Size * 0.5f;
		GD.Print($"[EnemyController] centered at {GlobalPosition}");
		player = FindPlayerInCurrentScene();
		
	}

	public override void _Process(double delta)
	{
		timer += delta;
		if (timer >= SpawnPeriod)
		{
			timer = 0.0;
			TrySpawnOne();
		}
	}

	private Player FindPlayerInCurrentScene()
	{
		var current = GetTree().CurrentScene;
		if (current == null)
			return null;

		var stack = new Stack<Node>();
		stack.Push(current);
		while (stack.Count > 0)
		{
			var node = stack.Pop();
			if (node is Player p)
				return p;
			foreach (var child in node.GetChildren())
			{
				if (child is Node c)
					stack.Push(c);
			}
		}
		return null;
	}

	private void TrySpawnOne()
	{
		int count = 0;
		foreach (var child in GetChildren())
		{
			if (child is EnemyCharacter)
				count++;
		}

		if (EnemyScene == null)
		{
			GD.PrintErr("EnemyScene is not set");
			return;
		}

		var inst = EnemyScene.Instantiate();
		if (inst is Node2D enemyNode)
		{
			Node parentForEnemies = GetTree().CurrentScene ?? GetTree().Root;
			parentForEnemies.AddChild(enemyNode);
			if (player != null)
			{
				float angle = rng.RandfRange(0.0f, MathF.PI * 2.0f);
				float dist = rng.RandfRange(MinSpawnDistance, MaxSpawnDistance);
				var offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
				enemyNode.GlobalPosition = player.GlobalPosition + offset;
				GD.Print($"Spawned enemy at global pos {enemyNode.GlobalPosition} (dist={dist:F1})");
			}
			else
			{
				float halfX = SpawnArea.X / 2.0f;
				float halfY = SpawnArea.Y / 2.0f;
				float rx = rng.RandfRange(-halfX, halfX);
				float ry = rng.RandfRange(-halfY, halfY);
				enemyNode.Position = new Vector2(rx, ry);
				GD.Print($"Spawned enemy at pos {enemyNode.Position}");
			}
		}
	}
}
