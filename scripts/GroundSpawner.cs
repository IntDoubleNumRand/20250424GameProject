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

		// Random number of patches (2–5)
		int patchCount = rng.Next(2, 6);

		// Minimum separation so they don't overlap
		float spacing = 20f;
		
		var positions = new List<Vector3>();

		GD.Print(patchCount);
		for (int i = 0; i < patchCount; i++)
		{
			// Pick a random position, retry until it's far enough from others
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

			// Generate a unique path for this patch
			var path = new Path();

			// Build one ground patch with its own path
			GroundBuilder.Create(this)
				.SetWidth(15)
				.SetDepth(15)
				.SetCellSize(1.0f)
				.SetEnableHill(true)
				.SetHillHeight(3.5f)
				.SetPlateauT(0.7f)
				.SetFalloffExponent(0.6f)
				.SetColor(new Color(0.9f, 0.1f, 0.1f))
				.SetLocation(pos)
				.SetPathCells(path.AllPathCells)
				.Build();
		}
	}
}
