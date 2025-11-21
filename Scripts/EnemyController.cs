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
	public float MaxSpawnDistance = 1200.0f;

	[Export]
	public double SpawnPeriod = 2.0;

	private Area2D spawnAreaNode = null;
	private CollisionPolygon2D spawnPolygon = null;
	private Vector2[] spawnPolygonPoints = null;
    
	private bool IsPointInsideSpawnArea(Vector2 globalPos)
	{
		if (spawnPolygon == null || spawnPolygonPoints == null)
			return false;
		var local = spawnPolygon.ToLocal(globalPos);
		return PointInPolygon(spawnPolygonPoints, local);
	}

	private bool PointInPolygon(Vector2[] poly, Vector2 point)
	{
		int n = poly.Length;
		bool inside = false;
		for (int i = 0, j = n - 1; i < n; j = i++)
		{
			var xi = poly[i].X; var yi = poly[i].Y;
			var xj = poly[j].X; var yj = poly[j].Y;
			bool intersect = ((yi > point.Y) != (yj > point.Y)) &&
							 (point.X < (xj - xi) * (point.Y - yi) / (yj - yi + 1e-9f) + xi);
			if (intersect)
				inside = !inside;
		}
		return inside;
	}

	[Export]
	public double DifficultyIncreasePeriod = 10.0;
	private double timer = 0.0;
	private double difficultyTimer = 0.0;
	private RandomNumberGenerator rng = new RandomNumberGenerator();
	private Player player = null;

	private int templateHp = 5;

	public override void _Ready()
	{
		rng.Randomize();
		var rect = GetViewport().GetVisibleRect();
		GlobalPosition = rect.Size * 0.5f;
		GD.Print($"[EnemyController] centered at {GlobalPosition}");
		player = FindPlayerInCurrentScene();
		// Find an Area2D child that defines the spawn region (first one)
		foreach (var child in GetChildren())
		{
			if (child is Area2D a)
			{
				spawnAreaNode = a;
				break;
			}
		}
		if (spawnAreaNode != null)
		{
			spawnPolygon = spawnAreaNode.GetNodeOrNull<CollisionPolygon2D>("CollisionPolygon2D");
			if (spawnPolygon == null)
			{
				throw new Exception("EnemyController: Spawn Area found but no CollisionPolygon2D inside it");
			}
			if (spawnPolygon != null)
			{
				spawnPolygonPoints = spawnPolygon.Polygon;
			}
			else
			{
				GD.Print("[EnemyController] Spawn Area found but no CollisionPolygon2D inside it");
			}
		}
		else
		{
			throw new Exception("EnemyController: No Spawn Area (Area2D) child found");
		}
	}

	public override void _Process(double delta)
	{
		if (GameState.IsGameOver)
			return;
		timer += delta;
		difficultyTimer += delta;
		if (difficultyTimer >= DifficultyIncreasePeriod)
		{
			difficultyTimer = 0.0;
			if (SpawnPeriod > 0.2)
			{
				SpawnPeriod -= 0.1;
				templateHp += 10;
				GD.Print($"Increased difficulty");
			}

		}
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
		if (EnemyScene == null)
		{
			GD.PrintErr("EnemyScene is not set");
			return;
		}

		var inst = EnemyScene.Instantiate();
		GD.Print($"[EnemyController] instantiated enemy: {inst} & type {inst.GetType()}");
		if (inst is EnemyCharacter enemy)
		{
			AddChild(enemy);
			enemy.SetTemplateHealth(templateHp);

			Vector2? spawnPos = null;
			if (player != null && spawnPolygonPoints != null)
			{
				for (int i = 0; i < 128; i++)
				{
					float angle = rng.RandfRange(0.0f, MathF.PI * 2.0f);
					float dist = rng.RandfRange(MinSpawnDistance, MaxSpawnDistance);
					var candidate = player.GlobalPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
					if (IsPointInsideSpawnArea(candidate))
					{
						spawnPos = candidate;
						break;
					}
				}
			}

			if (spawnPos == null && spawnPolygonPoints != null)
			{
				var boundsMin = new Vector2(float.MaxValue, float.MaxValue);
				var boundsMax = new Vector2(float.MinValue, float.MinValue);
				for (int i = 0; i < spawnPolygonPoints.Length; i++)
				{
					var local = spawnPolygonPoints[i];
					var global = spawnPolygon.ToGlobal(local);
					boundsMin.X = MathF.Min(boundsMin.X, global.X);
					boundsMin.Y = MathF.Min(boundsMin.Y, global.Y);
					boundsMax.X = MathF.Max(boundsMax.X, global.X);
					boundsMax.Y = MathF.Max(boundsMax.Y, global.Y);
				}
				for (int i = 0; i < 128; i++)
				{
					float rx = rng.RandfRange(boundsMin.X, boundsMax.X);
					float ry = rng.RandfRange(boundsMin.Y, boundsMax.Y);
					var cand = new Vector2(rx, ry);
					if (IsPointInsideSpawnArea(cand))
					{
						if (player == null || cand.DistanceTo(player.GlobalPosition) >= MinSpawnDistance)
						{
							spawnPos = cand;
							break;
						}
					}
				}
			}

			if (spawnPos != null)
			{
				enemy.GlobalPosition = spawnPos.Value;
				GD.Print($"Spawned enemy at global pos {enemy.GlobalPosition}");
			}
			else
			{
				GD.PrintErr("Failed to find valid spawn position for enemy");
				enemy.QueueFree();
			}
		}
	}
}
