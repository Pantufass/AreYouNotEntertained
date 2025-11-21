using Godot;
using System;
using System.Linq;

public partial class LookArea : Area2D
{
	private Vector2? lastLookAtVector = null;

	public Vector2? GetLookAtVector()
	{
		var overlaps = GetOverlappingBodies().OfType<Node>().Where(n => n is EnemyCharacter).Cast<EnemyCharacter>();
		var closest = overlaps.OrderBy(e => e.GlobalPosition.DistanceTo(this.GlobalPosition)).FirstOrDefault();
		if (closest != null)
		{
			var vec = (closest.GlobalPosition - this.GlobalPosition).Normalized();
			lastLookAtVector = vec;
			return vec;
		}
		lastLookAtVector = null;
		return null;
	}

	public void OnSlash()
	{
		var vec = GetLookAtVector();
		if (vec != null)
		{
			LookAt(GlobalPosition + vec.Value);
		}
	}

	public Vector2? LastLookAtVector => lastLookAtVector;
}
