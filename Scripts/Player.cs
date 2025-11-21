using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Character
{
    public static Player Instance { get; private set; }

    public int Score {get; private set;} = 0;
    public double AttackSpeed {get; private set;} = 2.0;
    public int Level {get; private set;} = 1;
    public Dictionary<int,float> LevelProgression {get;} = new Dictionary<int,float>()
    {
        {1, 0f},
        {2, 6f},
        {3, 12f},
        {4, 20f},
        {5, 30f},
        {6, 50f},
        {7, 75f},
        {8, 100f},
        {9, 140f},
        {10, 200f}
    };
    public static event Action<float> OnKill;
    public static event Action<int> OnLevelUp;
    public static event Action OnGameOver;
    public Player() : base(new int[]{ 350, 10, 10, 5, 0 })
    {
        OnKill += GetExp;
        OnLevelUp += LevelUp;
    }

	public override void _Ready()
	{
        Instance = this;
		
	}

    public override void _ExitTree()
    {
        if (Instance == this) Instance = null;
        OnKill -= GetExp;
        OnLevelUp -= LevelUp;
    }
	public override void _Process(double delta)
	{
        if (GameState.IsGameOver)
            return;

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
            OnGameOver?.Invoke();
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

    // Allow external systems (UI) to modify player stats through methods
    public void ModifySpeed(float delta)
    {
        Speed += delta;
        GD.Print($"[Player] Speed modified by {delta}, now {Speed}");
    }

    public void ModifyAttack(float delta)
    {
        Attack += delta;
        GD.Print($"[Player] Attack modified by {delta}, now {Attack}");
    }

    public void ModifyHealth(float delta)
    {
        Health += delta;
        GD.Print($"[Player] Health modified by {delta}, now {Health}");
    }

    public void ModifyDefense(float delta)
    {
        Defense += delta;
        GD.Print($"[Player] Defense modified by {delta}, now {Defense}");
    }
}
