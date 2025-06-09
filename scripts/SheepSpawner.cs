using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class SheepSpawner : Node3D
{
	private readonly Lazy<List<Vector3>> _positions;
	private readonly int _initialNumber;
	private readonly Vector3 _goal;

	public SheepSpawner(ISpawnerStrategy strategy, int initialNumber, Vector3 goal)
	{
		_initialNumber = initialNumber;
		_positions = new Lazy<List<Vector3>>(() => strategy.GenerateSpawnPositions());
		_goal = goal;
	}

	public override void _Ready()
	{
		var scene = (PackedScene)GD.Load("res://scenes/SheepPrefab.tscn");
		int num = _initialNumber;
		foreach (var pos in _positions.Value)
			SpawnSheep(scene, pos, num++);
		var camera = GetTree().Root.GetNode<Camera3D>("Stage/Camera3D");
		foreach (var sheep in GetTree().GetNodesInGroup("Sheep").OfType<Sheep>())
		{
			sheep.SetGoal(_goal);
		}
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
