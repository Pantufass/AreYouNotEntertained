using Godot;
using System;

public partial class CanvasLayer : Godot.CanvasLayer
{
    [Export]
    public string FirstScenePath { get; set; } = "res://Scenes/firstscene.tscn";

    private ColorRect gameoverRect;
    private RichTextLabel gameoverLabel;
    private Button restartButton;

    public override void _Ready()
    {
        gameoverRect = GetNodeOrNull<ColorRect>("gameover");
        gameoverLabel = GetNodeOrNull<RichTextLabel>("RichTextLabel");
        restartButton = GetNodeOrNull<Button>("Button");

        // hide UI at start
        if (gameoverRect != null)
        {
            gameoverRect.Visible = false;
            if (gameoverRect is Control cr)
                cr.MouseFilter = Control.MouseFilterEnum.Ignore;
        }
        if (gameoverLabel != null)
        {
            gameoverLabel.Visible = false;
        }
        if (restartButton != null)
        {
            restartButton.Visible = false;
            restartButton.Pressed += OnRestartPressed;
        }

        Player.OnGameOver += HandleGameOver;
    }

    public void HandleGameOver()
    {
        GD.Print("Game Over!");

        // show UI
        if (gameoverRect != null) gameoverRect.Visible = true;
        if (gameoverLabel != null) gameoverLabel.Visible = true;
        if (restartButton != null) restartButton.Visible = true;

    // mark global game-over so gameplay stops (don't pause the tree so UI stays interactive)
    GameState.IsGameOver = true;
    }

    private void OnRestartPressed()
    {
        GD.Print("Restarting game...");
        // clear game over flag so gameplay resumes in the newly loaded scene
        GameState.IsGameOver = false;
        var tree = GetTree();

        var reloadMethod = tree.GetType().GetMethod("ReloadCurrentScene");
        if (reloadMethod != null)
        {
            reloadMethod.Invoke(tree, null);
            return;
        }

        var current = tree.CurrentScene;
        if (current != null)
        {
            var type = current.GetType();
            var prop = type.GetProperty("SceneFilePath") ?? type.GetProperty("Filename");
            if (prop != null)
            {
                var pathObj = prop.GetValue(current);
                if (pathObj is string path && !string.IsNullOrEmpty(path))
                {
                    tree.ChangeSceneToFile(path);
                    return;
                }
            }
        }

        if (!string.IsNullOrEmpty(FirstScenePath))
        {
            tree.ChangeSceneToFile(FirstScenePath);
        }
    }
}
