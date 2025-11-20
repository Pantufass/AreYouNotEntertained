using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Character
{
    public int Score {get; private set;} = 0;
    public double AttackSpeed {get; private set;} = 2.0;
    public int Level {get; private set;} = 1;
    public Dictionary<int,float> LevelProgression {get;} = new Dictionary<int,float>()
    {
        {1, 0f},
        {2, 10f},
        {3, 25f},
        {4, 45f},
        {5, 70f},
        {6, 100f},
        {7, 130f},
        {8, 175f},
        {9, 220f},
        {10, 270f}
    };
    public static event Action<float> OnKill;
    public static event Action<int> OnLevelUp;
    public Player() : base(new int[]{ 350, 10, 10, 5, 0 })
    {
        OnKill += GetExp;
        OnLevelUp += LevelUp;
    }

    public override void _Ready()
    {
        
    }
    public override void _Process(double delta)
    {
        var input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Velocity = input * Speed;
        MoveAndSlide();
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        GD.Print($"[Player] took {amount} damage, health now {Health}");
        if (Health <= 0)
        {
            GD.Print("[Player] died (QueueFree)");
            //Game Over
        }
    }

    public static void OnKillEnemy(float exp)
    {
        OnKill?.Invoke(exp);
    }
    public void GetExp(float exp)
    {
        Score += 1;
        Experience += exp;
        GD.Print($"[Player] gained {exp} experience, total now {Experience}");
        if(LevelProgression.ContainsKey(Level + 1) && Experience >= LevelProgression[Level + 1])
        {
            OnLevelUp?.Invoke(Level);
        }
    }

    public void LevelUp(int exp)
    {
        Level += 1;
        GD.Print($"[Player] leveled up to level {Level}!");
    }
}
