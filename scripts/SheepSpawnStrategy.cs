using Godot;
using System.Collections.Generic;

public class SheepSpawnStrategy : ISpawnerStrategy
{
	private readonly RandomNumberGenerator _rng = new RandomNumberGenerator();
	private readonly float _minX = -50f, _maxX = 50f, _minZ = 0f, _maxZ = 50f;
	private readonly float _exclMinX = -10f, _exclMaxX = 10f, _exclMinZ = 10f, _exclMaxZ = 20f;
	private readonly int _count;

	public SheepSpawnStrategy(int count = 1)
	{
		_rng.Randomize();
		_count = count;
	}

	public List<Vector3> GenerateSpawnPositions()
	{
		var positions = new List<Vector3>();
		for (int i = 0; i < _count; i++)
		{
			Vector3 pos;
			int attempts = 0;
			do
			{
				float x = _rng.RandfRange(_minX, _maxX);
				float z = _rng.RandfRange(_minZ, _maxZ);
				pos = new Vector3(x, 1.201f, z);
				attempts++;
			} while (IsInExclusion(pos) && attempts < 20);
			positions.Add(pos);
		}
		GD.Print($"Sheeps: {positions.Count}");
		
		return positions;
	}

	private bool IsInExclusion(Vector3 pos)
	{
		return pos.X > _exclMinX && pos.X < _exclMaxX && pos.Z > _exclMinZ && pos.Z < _exclMaxZ;
	}
}
