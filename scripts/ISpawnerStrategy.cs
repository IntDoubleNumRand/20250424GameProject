using System.Collections.Generic;
using Godot;

public interface ISpawnerStrategy
{
	List<Vector3> GenerateSpawnPositions();
}
