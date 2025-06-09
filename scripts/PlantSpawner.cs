using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class PlantSpawner : Node3D
{
	private readonly Lazy<List<Vector3>> _positions;

	public PlantSpawner(ISpawnerStrategy strategy)
	{
		_positions = new Lazy<List<Vector3>>(() => strategy.GenerateSpawnPositions());
	}

	public override void _Ready()
	{
		var rng = new RandomNumberGenerator();
		rng.Randomize();

		foreach (var pos in _positions.Value)
		{
			var type = (PlantType)rng.RandiRange(0, 3);
			var fly = PlantFlyweightFactory.Get(type);
			var plant = fly.Instantiate(pos, 5f);
			AddChild(plant);
		}
	}
}
