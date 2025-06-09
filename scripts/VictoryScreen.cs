using Godot;

public partial class VictoryScreen : Control
{
	public override void _Ready()
	{
		var label = new Label { Text = "Victory!" };
		label.Position = new Vector2(300, 200);
		AddChild(label);

		var button = new Button { Text = "Back to Start", Name = "ReturnButton" };
		button.Position = new Vector2(300, 300);
		button.Pressed += () =>
		{
			SheepManager.Instance?.ResetGameState();
			GetTree().ChangeSceneToFile("res://scenes/StartScreen.tscn");
		};
		AddChild(button);
		
		var cheerSound = new AudioStreamPlayer();
		cheerSound.Stream = GD.Load<AudioStream>("res://assets/mixkit-birthday-crowd-party-cheer-531.mp3");
		AddChild(cheerSound);
		cheerSound.Play();
	}
}
