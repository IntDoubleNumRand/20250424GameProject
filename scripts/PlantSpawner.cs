using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class PlantSpawner : Node3D
{
	private readonly Lazy<List<Vector3>> _positions;
	private readonly PackedScene _flowerYellow;
	private readonly PackedScene _flowerRed;
	private readonly PackedScene _flowerPurple;
	private readonly PackedScene _grass;

	public PlantSpawner(ISpawnerStrategy strategy)
	{
		_positions = new Lazy<List<Vector3>>(() => strategy.GenerateSpawnPositions());
		_flowerYellow = GD.Load<PackedScene>("res://scenes/FlowerYellow.tscn");
		_flowerRed = GD.Load<PackedScene>("res://scenes/FlowerRed.tscn");
		_flowerPurple = GD.Load<PackedScene>("res://scenes/FlowerPurple.tscn");
		_grass = GD.Load<PackedScene>("res://scenes/Grass.tscn");
	}

	public override void _Ready()
	{
		var rng = new RandomNumberGenerator();
		rng.Randomize();

		foreach (var pos in _positions.Value)
		{
			PackedScene scene;
			switch (rng.RandiRange(0, 3))
			{
				case 0: scene = _flowerYellow; break;
				case 1: scene = _flowerRed; break;
				case 2: scene = _flowerPurple; break;
				default: scene = _grass; break;
			}

			var plant = scene.Instantiate<Node3D>();
			plant.Position = pos;
			plant.Scale = new Vector3(5f,5f,5f);
			plant.AddToGroup("Plant");
			AddChild(plant);
		}
	}
}
