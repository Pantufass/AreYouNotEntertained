using Godot;
using System;

public partial class Player : CharacterBody2D
{
    
    public int Speed {get; private set;} = 300;
    public int Health {get; private set;} = 100;
    public int Attack {get; private set;} = 10;
    public int Defense {get; private set;} = 5;
    public int Experience {get; private set;} = 0;

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
