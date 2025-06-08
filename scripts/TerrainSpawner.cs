using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class TerrainSpawner : Node3D
{
	private readonly Lazy<List<Vector3>> _positions;

	public TerrainSpawner(ISpawnerStrategy strategy)
	{
		_positions = new Lazy<List<Vector3>>(() => strategy.GenerateSpawnPositions());
	}

	public override void _Ready()
	{
		foreach (var pos in _positions.Value)
			SpawnPatch(pos);
	}

	private void SpawnPatch(Vector3 pos)
	{
		var rng = new RandomNumberGenerator();
		rng.Randomize();

		TerrainBuilder.Create(this)
			.SetWidth((int)rng.RandiRange(30, 39))
			.SetDepth((int)rng.RandiRange(10, 49))
			.SetCellSize(1.0f)
			.SetEnableHill(true)
			.SetHillHeight(rng.RandfRange(5f, 13f))
			.SetPlateauT(0.7f)
			.SetFalloffExponent(0.6f)
			.SetLocation(pos)
			.Build();
	}
}
