using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class GroundSpawner : Node3D
{
	public override void _Ready()
	{
		SpawnGroundPatches();
	}
	
	private void SpawnGroundPatches()
	{
		var rng = new RandomNumberGenerator();
		rng.Randomize();

		Vector3 camPos = new Vector3(5f, 5f, 81f);
		float excludeRadius = 1f;

		int patchCount = (int)rng.RandiRange(6, 14);
		float spacing = 8f;

		var positions = new List<Vector3>();
		GD.Print($"Patch count: {patchCount}");
		for (int i = 0; i < patchCount; i++)
		{
			Vector3 pos;
			int attempts = 0;
			do
			{
				float x = rng.RandfRange(-spacing * 4, spacing * 4);
				float z = rng.RandfRange(-spacing * 4, spacing * 4);
				pos = new Vector3(x, -1.0f, z);
				attempts++;
			}
			while (
				(positions.Exists(p => p.DistanceTo(pos) < spacing) || (pos - camPos).Length() < excludeRadius)
				&& attempts < 100
			);
			positions.Add(pos);

			GroundBuilder.Create(this)
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
}
