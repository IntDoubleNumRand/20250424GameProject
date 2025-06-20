using Godot;
using System;

public partial class StartScreen : Control
{
	public override void _Ready()
	{
		var sheepLabel = new Label { Text = "Sheep Count:" };
		sheepLabel.Position = new Vector2(50, 50);
		AddChild(sheepLabel);

		var sheepInput = new LineEdit { Name = "SheepInput", Text = "5" };
		sheepInput.Position = new Vector2(200, 50);
		AddChild(sheepInput);

		var wolfLabel = new Label { Text = "Wolf Count:" };
		wolfLabel.Position = new Vector2(50, 100);
		AddChild(wolfLabel);

		var wolfInput = new LineEdit { Name = "WolfInput", Text = "2" };
		wolfInput.Position = new Vector2(200, 100);
		AddChild(wolfInput);

		var startButton = new Button { Name = "StartButton", Text = "Start Game" };
		startButton.Position = new Vector2(100, 160);
		startButton.Pressed += OnStartPressed;
		AddChild(startButton);
		
		var demoVictoryButton = new Button { Name = "DemoVictoryButton", Text = "Demo Victory" };
		demoVictoryButton.Position = new Vector2(100, 200);
		demoVictoryButton.Pressed += OnDemoVictoryPressed;
		AddChild(demoVictoryButton);
		
		var demoGameOverButton = new Button { Name = "DemoGameOverButton", Text = "Demo Game Over" };
		demoGameOverButton.Position = new Vector2(250, 200);
		demoGameOverButton.Pressed += OnDemoGameOverPressed;
		AddChild(demoGameOverButton);
	}
	
	private void OnStartPressed()
	{
		int sheepCount = int.Parse(GetNode<LineEdit>("SheepInput").Text);
		int wolfCount = int.Parse(GetNode<LineEdit>("WolfInput").Text);
		
		var gameSettings = new Godot.Collections.Dictionary<string, int>
		{
			{ "sheep", sheepCount },
			{ "wolf", wolfCount }
		};
		
		GetTree().Root.SetMeta("GameSettings", gameSettings);
		GetTree().ChangeSceneToFile("res://scenes/Stage.tscn");
	}
	
	private void OnDemoVictoryPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/VictoryScreen.tscn");
	}
	
	private void OnDemoGameOverPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/GameOverScreen.tscn");
	}
}
