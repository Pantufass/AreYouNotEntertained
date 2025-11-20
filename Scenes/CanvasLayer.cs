using Godot;
using System;

public partial class CanvasLayer : Godot.CanvasLayer
{
    public override void _Ready()
    {
        Player.OnGameOver += HandleGameOver;
    }

    public void HandleGameOver()
    {
        GD.Print("Game Over!");
    }
}
