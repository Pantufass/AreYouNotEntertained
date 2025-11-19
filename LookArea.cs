using Godot;
using System;

public partial class LookArea : Area2D
{
    public override void _Process(double delta)
    {
      var enemies = GetOverlappingBodies();
      foreach (var enemy in enemies)
      {
        if (enemy is EnemyCharacter enemyCharacter)
        {
            LookAt(enemyCharacter.GlobalPosition);    
        }
      }
    }
}
