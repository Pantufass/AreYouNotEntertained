using Godot;
using System;

public partial class LevelUpUI : CanvasLayer
{
    private ColorRect overlay;
    private TextureRect upImg;
    private TextureRect downImg;
    private RichTextLabel label;
    private RandomNumberGenerator rng = new RandomNumberGenerator();

    public override void _Ready()
    {
        rng.Randomize();
        overlay = GetNodeOrNull<ColorRect>("ColorRect");
        upImg = GetNodeOrNull<TextureRect>("up");
        downImg = GetNodeOrNull<TextureRect>("down");
        label = GetNodeOrNull<RichTextLabel>("RichTextLabel");

        if (overlay != null) overlay.Visible = false;
        if (upImg != null)
        {
            upImg.Visible = false;
            upImg.GuiInput += OnUpGuiInput;
        }
        if (downImg != null)
        {
            downImg.Visible = false;
            downImg.GuiInput += OnDownGuiInput;
        }
        if (label != null) label.Visible = false;

        Player.OnLevelUp += OnLevelUp;
    }

    public override void _ExitTree()
    {
        // unsubscribe to avoid holding references to freed nodes across scene changes
        Player.OnLevelUp -= OnLevelUp;
        if (upImg != null) upImg.GuiInput -= OnUpGuiInput;
        if (downImg != null) downImg.GuiInput -= OnDownGuiInput;
    }

    private void OnLevelUp(int level)
    {
        GD.Print($"[LevelUpUI] Level up received: {level}");
        ShowUI();
    }

    private void ShowUI()
    {
        if (overlay != null) overlay.Visible = true;
        if (upImg != null) upImg.Visible = true;
        if (downImg != null) downImg.Visible = true;
        if (label != null)
        {
            label.Visible = true;
            label.Text = "Choose Buff (Up) or Debuff (Down)";
        }
        // pause gameplay via GameState flag so UI remains interactive
        GameState.IsGameOver = true;
    }

    private void HideUI()
    {
        if (overlay != null) overlay.Visible = false;
        if (upImg != null) upImg.Visible = false;
        if (downImg != null) downImg.Visible = false;
        if (label != null) label.Visible = false;
        GameState.IsGameOver = false;
    }

    private void OnUpGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mbe && mbe.Pressed && mbe.ButtonIndex == MouseButton.Left)
        {
            ApplyChoice(true);
        }
    }

    private void OnDownGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mbe && mbe.Pressed && mbe.ButtonIndex == MouseButton.Left)
        {
            ApplyChoice(false);
        }
    }

    private Player FindPlayerInCurrentScene()
    {
        var current = GetTree().CurrentScene;
        if (current == null) return null;
        foreach (var child in current.GetChildren())
        {
            if (child is Player p) return p;
            if (child is Node node)
            {
                var found = FindPlayerRecursive(node);
                if (found != null) return found;
            }
        }
        return null;
    }

    private Player FindPlayerRecursive(Node node)
    {
        foreach (var c in node.GetChildren())
        {
            if (c is Player p) return p;
            if (c is Node n)
            {
                var found = FindPlayerRecursive(n);
                if (found != null) return found;
            }
        }
        return null;
    }

    private void ApplyChoice(bool choseBuff)
    {
        var player = Player.Instance ?? FindPlayerInCurrentScene();
        if (player == null)
        {
            GD.PrintErr("[LevelUpUI] Could not find Player to apply buff/debuff");
            HideUI();
            return;
        }

        if (choseBuff)
        {
            // pick a random buff
            int choice = rng.RandiRange(0, 2);
            int value;
            switch (choice)
            {
                case 0:
                    value = rng.RandiRange(0, 50);
                    player.ModifySpeed(value);
                    break;
                case 1:
                    value = rng.RandiRange(0, 4);
                    player.ModifyAttack(value);
                    break;
                case 2:
                    value = rng.RandiRange(0, 20);
                    player.ModifyHealth(value);
                    break;
            }
        }
        else
        {
            // pick a random debuff
            int choice = rng.RandiRange(0, 1);
            switch (choice)
            {
                case 0:
                    player.ModifySpeed(-30f);
                    break;
                case 1:
                    player.ModifyAttack(-1f);
                    break;
            }
        }
        HideUI();
    }
}
