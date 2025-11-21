using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Character
{
	private static AnimatedSprite2D animation = null;

    // attack animation lock: prevent animation changes for this duration after attacking
    private bool isAttackLocked = false;
    private double attackLockTimer = 0.0;
    public double attackLockDuration = 0.2;
	
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
	public static event Action OnGameOver;
    public static Player Instance { get; private set; }
	public Player() : base(new int[]{ 350, 10, 10, 5, 0 })
	{
		OnKill += GetExp;
		OnLevelUp += LevelUp;
	}


	public override void _Ready()
	{
        Instance = this;
		if (animation == null)
		{
			animation = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
			//animation.play("Move");
		}
	}

    public override void _ExitTree()
    {
        if (Instance == this) Instance = null;
        OnKill -= GetExp;
        OnLevelUp -= LevelUp;
    }
		
	
	public override void _Process(double delta)
	{
        if (GameState.IsGameOver) return;


        var input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        if (isAttackLocked)
        {
            attackLockTimer -= delta;
            if (attackLockTimer <= 0)
            {
                isAttackLocked = false;
                attackLockTimer = 0.0;
            }
        }

        if (!isAttackLocked)
        {
            if(input != Vector2.Zero)
            {
                if (animation != null && input.X != 0) animation.FlipH = input.X < 0;
                animation.Play("move");
            }
            else
            {
                animation.Play("idle");
            }
        }
        Velocity = input * Speed;
        MoveAndSlide();
	}

    public void OnSlash(Vector2 direction)
    {
        GD.Print($"[Player] OnSlash called with direction {direction}");
        if (animation != null && direction.X != 0){
            animation.FlipH = direction.X < 0;
        }
        if(animation != null) animation.Play("attack");
            
        isAttackLocked = true;
        attackLockTimer = attackLockDuration;
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
