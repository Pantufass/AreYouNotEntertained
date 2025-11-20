using Godot;
using System;

public partial class Character : CharacterBody2D
{
    
    public float Speed {get; protected set;} = 100f;
    public float Health {get; protected set;} = 1f;
    public float Attack {get; protected set;} = 1f;
    public float Defense {get; protected set;} = 1f;
    public float Experience {get; protected set;} = 1f;

    public Character(int[] stats)
    {
        Speed = stats[0];
        Health = stats[1];
        Attack = stats[2]; 
        Defense = stats[3];
        Experience = stats[4];
    }
    public Character() {}

    public override void _Ready()
    {
        
    }
    public override void _Process(double delta)
    {
        var input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Velocity = input * Speed;
        MoveAndSlide();
    }
}
