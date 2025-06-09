using Godot;

public partial class GameOverScreen : Control
{
	public override void _Ready()
	{
		var label = new Label { Text = "Game Over" };
		label.Position = new Vector2(300, 200);
		AddChild(label);

		var button = new Button { Text = "Try Again", Name = "ReturnButton" };
		button.Position = new Vector2(300, 300);
		button.Pressed += () =>
		{
			SheepManager.Instance?.ResetGameState();
			GetTree().ChangeSceneToFile("res://scenes/StartScreen.tscn");
		};
		AddChild(button);
	}
}
