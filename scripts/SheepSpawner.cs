using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class SheepSpawner : Node3D
{
	private readonly Lazy<List<Vector3>> _positions;
	private readonly int _initialNumber;

	public SheepSpawner(ISpawnerStrategy strategy, int initialNumber)
	{
		_initialNumber = initialNumber;
		_positions = new Lazy<List<Vector3>>(() => strategy.GenerateSpawnPositions());
	}

	public override void _Ready()
	{
		var scene = (PackedScene)GD.Load("res://scenes/SheepPrefab.tscn");
		int num = _initialNumber;
		foreach (var pos in _positions.Value)
			SpawnSheep(scene, pos, num++);
	}

	private void SpawnSheep(PackedScene scene, Vector3 pos, int number)
	{
		GD.Print($"[SheepSpawner] Spawning sheep{number} at {pos}");
		var sheep = (CharacterBody3D)scene.Instantiate();
		sheep.Name = "Sheep" + number;
		AddChild(sheep);
		sheep.Scale = new Vector3(1.3f,1.3f,1.3f);
		sheep.GlobalPosition = pos;
	}
}
