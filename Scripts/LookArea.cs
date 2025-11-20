using Godot;
using System;

public partial class LookArea : Area2D
{
    public void OnSlash()
    {
      GD.Print("LookArea detected slash!");
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
