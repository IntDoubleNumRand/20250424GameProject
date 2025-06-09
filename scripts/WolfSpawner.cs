using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class WolfSpawner : Node3D
{
	private readonly Lazy<List<Vector3>> _positions;
	private readonly int _initialNumber;

	public WolfSpawner(ISpawnerStrategy strategy,int initialNumber)
	{
		_initialNumber = initialNumber;
		_positions = new Lazy<List<Vector3>>(() => strategy.GenerateSpawnPositions());
	}

	public override void _Ready()
	{
		var scene = (PackedScene)GD.Load("res://scenes/WolfPrefab.tscn");
		int num = _initialNumber;
		foreach (var pos in _positions.Value)
			SpawnWolf(scene,pos,num++);
	}

	private void SpawnWolf(PackedScene scene,Vector3 pos,int number)
	{
		var wolf = (CharacterBody3D)scene.Instantiate();
		wolf.Name = "Wolf"+number;
		AddChild(wolf);
		wolf.GlobalPosition = pos;
	}
}
