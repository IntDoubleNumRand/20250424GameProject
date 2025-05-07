// GroundSpawner.cs
using Godot;
using System;

[Tool]
public partial class GroundSpawner : Node3D
{
	public void SpawnGroundPatches()
	{
		var rng = new Random();
		int patchCount = rng.Next(2, 6);
		float patchWidth = 15 * 1.0f;

		for (int i = 0; i < patchCount; i++)
		{
			Vector3 pos = new(i * patchWidth, 0f, 0f);
			GroundBuilder.Create(this)
				.SetWidth(15)
				.SetDepth(15)
				.SetCellSize(1.0f)
				.SetEnableHill(true)
				.SetHillHeight(3.5f)
				.SetFalloffExponent(0.6f)
				.SetLocation(pos)
				.Build();
		}
	}
}
