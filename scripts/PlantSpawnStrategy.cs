using Godot;
using System;
using System.Collections.Generic;

public class PlantSpawnStrategy : ISpawnerStrategy
{
	private readonly Vector3 _cameraOrigin;
	private readonly List<Vector3> _terrainPositions;
	private readonly int _minCount;
	private readonly int _maxCount;
	private readonly float _minX, _maxX, _minZ, _maxZ;
	private readonly float _excludeRadius;
	private readonly float _plantRadius;
	private readonly RandomNumberGenerator _rng = new RandomNumberGenerator();

	public PlantSpawnStrategy(
		Vector3 cameraOrigin,
		List<Vector3> terrainPositions,
		float minX = -50f,
		float maxX = 50f,
		float minZ = 0f,
		float maxZ = 50f,
		int minCount = 20,
		int maxCount = 40,
		float excludeRadius = 2f,
		float plantDiameter = 1f)
	{
		_cameraOrigin = cameraOrigin;
		_terrainPositions = terrainPositions;
		_minX = minX;
		_maxX = maxX;
		_minZ = minZ;
		_maxZ = maxZ;
		_minCount = minCount;
		_maxCount = maxCount;
		_excludeRadius = excludeRadius;
		_plantRadius = plantDiameter * 0.5f;
		_rng.Randomize();
	}

	public List<Vector3> GenerateSpawnPositions()
	{
		int count = (int)_rng.RandiRange(_minCount, _maxCount);
		var positions = new List<Vector3>();
		float minSeparation = _plantRadius * 2f;

		for (int i = 0; i < count; i++)
		{
			Vector3 candidate;
			int tries = 0;

			do
			{
				var x = _rng.RandfRange(_minX, _maxX);
				var z = _rng.RandfRange(_minZ, _maxZ);
				candidate = new Vector3(x, 0.5f, z);
				tries++;
			}
			while (
				(candidate - _cameraOrigin).Length() < _excludeRadius
				|| IsTooCloseToTerrain(candidate, minSeparation)
				|| positions.Exists(p => p.DistanceTo(candidate) < minSeparation)
				&& tries < 20
			);

			positions.Add(candidate);
		}

		return positions;
	}

	private bool IsTooCloseToTerrain(Vector3 pos, float threshold)
	{
		foreach (var tpos in _terrainPositions)
			if (tpos.DistanceTo(pos) < threshold)
				return true;
		return false;
	}
}
