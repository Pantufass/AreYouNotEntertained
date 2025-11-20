using Godot;
using System;
using System.Collections.Generic;

public partial class Slash : Area2D
{
  [Export]
  public double Period = 2.0;
  [Export]
  public double VisibleTime = 0.25;
  [Export]
  public bool StartVisible = false;

  [Export]
  public Player player;
  private CollisionShape2D collisionShape = null;
  private double timer = 0.0;
  private HashSet<EnemyCharacter> hitEnemies = new HashSet<EnemyCharacter>();
  private bool previousVisible = false;

  private LookArea FocusArea;
  private event Action TriggerSlash;
  public override void _Ready()
  {
    if(this.player == null)
    {
        var parentNode = GetParent();
        if (parentNode != null && parentNode.GetParent() is Player player)
        {
          this.player = player;
        }
        if (parentNode != null && parentNode is LookArea lookArea)
        {
          FocusArea = lookArea;
          TriggerSlash += FocusArea.OnSlash;
        }

    }

    GD.Print($"Slash ready, player: {player}");
    collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");

    if (VisibleTime > Period)
      VisibleTime = Period;

    Visible = StartVisible;
    if (collisionShape != null)
      collisionShape.Disabled = !StartVisible;
    timer = 0.0;
    previousVisible = Visible;
  }

  public override void _Process(double delta)
  {
    timer += delta;
    var t = timer % Period;
    bool shouldBeVisible = t < VisibleTime;


    if (shouldBeVisible != Visible)
    {
      GD.Print(TriggerSlash);
      TriggerSlash?.Invoke();
      Visible = shouldBeVisible;
      if (collisionShape != null)
      {
        collisionShape.Disabled = !shouldBeVisible;
      }
    }

    if (shouldBeVisible && !previousVisible)
    {
      hitEnemies.Clear();
    }
    previousVisible = shouldBeVisible;

    var enemies = GetOverlappingBodies();
    foreach (var enemy in enemies)
    {
      if (enemy is EnemyCharacter enemyCharacter)
      {
        if (!hitEnemies.Contains(enemyCharacter))
        {
          GD.Print($"Enemy hit");
          hitEnemies.Add(enemyCharacter);
          enemyCharacter.GetHit(player.Attack);
        }
      }
    }
  }

}
