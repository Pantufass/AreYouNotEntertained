using Godot;

public partial class StartScreen : Control
{
    [Export]
    public string FirstScenePath = "res://Scenes/firstscene.tscn";

    public override void _Ready()
    {
        var button = GetNode<Button>("MarginContainer/HBoxContainer/VBoxContainer/Button");
        button.Pressed += OnStartButtonPressed;
    }

    private void OnStartButtonPressed()
    {
        GetTree().ChangeSceneToFile(FirstScenePath);
    }
}
