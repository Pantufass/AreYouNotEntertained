using Godot;
using System;

public partial class Player : Character
{
    public Player() : base(new int[]{ 100, 100, 10, 5, 0 })
    {
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
}
