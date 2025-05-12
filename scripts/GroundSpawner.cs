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
		var rng = new Random();
		// Random number of patches 
		int patchCount = rng.Next(6, 15);
		// Minimum separation so they don't overlap
		float spacing = 20f;
		
		var positions = new List<Vector3>();
		GD.Print(patchCount);
		for (int i = 0; i < patchCount; i++)
		{
			Vector3 pos;
			int attempts = 0;
			do
			{
				float x = (float)rng.NextDouble() * spacing * 4 - spacing * 2;
				float z = (float)rng.NextDouble() * spacing * 4 - spacing * 2;
				pos = new Vector3(x, 0f, z);
				attempts++;
			}
			while (positions.Exists(p => p.DistanceTo(pos) < spacing) && attempts < 100);
			positions.Add(pos);

			GroundBuilder.Create(this)
				.SetWidth(rng.Next(5, 16))
				.SetDepth(rng.Next(5, 16))
				.SetCellSize(1.0f)
				.SetEnableHill(true)
				.SetHillHeight((float)rng.NextDouble() * 5f)
				.SetPlateauT(0.7f)
				.SetFalloffExponent(0.6f)
				.SetLocation(pos)
				.Build();
		}
	}
}
