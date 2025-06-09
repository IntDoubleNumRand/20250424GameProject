using Godot;
using System;
using System.Collections.Generic;

public class TerrainSpawnStrategy : ISpawnerStrategy
{
	private readonly Vector3 _camPos;
	private readonly WorldPathAdapter _pathAdapter;
	private readonly float _excludeRadius;
	private readonly int _minCount;
	private readonly int _maxCount;
	private readonly float _spacing;
	private readonly float _patchRadius;
	private readonly float _minX;
	private readonly float _maxX;
	private readonly float _minZ;
	private readonly float _maxZ;
	private readonly RandomNumberGenerator _rng = new RandomNumberGenerator();

	public TerrainSpawnStrategy(
		Vector3 camPos, WorldPathAdapter pathAdapter,
		float minX = -50f, float maxX = 50f, float minZ = 0f, float maxZ = 50f, 
		float excludeRadius = 3.5f, int minCount = 6, int maxCount = 14, 
		float spacing = 8f, float patchWidth = 10f)
	{
		_camPos = new Vector3(camPos.X, -0.1f, camPos.Z);
		_pathAdapter = pathAdapter;
		_minX = minX;
		_maxX = maxX;
		_minZ = minZ;
		_maxZ = maxZ;
		_excludeRadius = excludeRadius;
		_minCount = minCount;
		_maxCount = maxCount;
		_spacing = spacing;
		_patchRadius = patchWidth * 0.5f;
		_rng.Randomize();
	}

	public List<Vector3> GenerateSpawnPositions()
	{
		int patchCount = (int)_rng.RandiRange(_minCount, _maxCount);
		GD.Print($"Ground Patches: {patchCount}");
		var positions = new List<Vector3>();
		
		float minPathDistance = _spacing + _patchRadius;
		
		for (int i = 0; i < patchCount; i++)
		{
			Vector3 pos;
			int attempts = 0;
			
			do
			{
				float x = _rng.RandfRange(_minX, _maxX);
				float z = _rng.RandfRange(_minZ, _maxZ);
				pos = new Vector3(x, -1f, z);
				attempts++;
			}
			// retry if too close to camera, path, or existing patch
			while ((positions.Exists(p => p.DistanceTo(pos) < minPathDistance)
					|| (pos - _camPos).Length() < _excludeRadius
					|| IsNearPath(pos, minPathDistance))
				   && attempts < 20);

			positions.Add(pos);
		}
		return positions;
	}

	private bool IsNearPath(Vector3 pos, float threshold)
	{
		var pathPoints = _pathAdapter.GetFullPath();
		Vector2 p2D = new Vector2(pos.X, pos.Z);

		for (int i = 0; i < pathPoints.Count - 1; i++)
		{
			var wp1 = pathPoints[i];
			var wp2 = pathPoints[i + 1];
			Vector2 a = new Vector2(wp1.X, wp1.Z);
			Vector2 b = new Vector2(wp2.X, wp2.Z);

			if (DistancePointToSegment(p2D, a, b) < threshold)
				return true;
		}

		return false;
	}

	private float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
	{
		Vector2 ab = b - a;
		float abSquared = ab.LengthSquared();
		if (abSquared == 0f)
			return p.DistanceTo(a);

		float t = (p - a).Dot(ab) / abSquared;
		t = Mathf.Clamp(t, 0f, 1f);
		Vector2 projection = a + ab * t;
		return p.DistanceTo(projection);
	}
}
