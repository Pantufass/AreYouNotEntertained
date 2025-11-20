using Godot;
using System;
using System.Linq;

public partial class LookArea : Area2D
{
	public void OnSlash()
	{
	  var enemies = GetOverlappingBodies();
	  enemies.OrderBy(e => e.GlobalPosition.DistanceTo(this.GlobalPosition)).Reverse();
	  foreach (var enemy in enemies)
	  {
		if (enemy is EnemyCharacter enemyCharacter)
		{
			LookAt(enemyCharacter.GlobalPosition);
			break;    
		}
	  }
	}
}
