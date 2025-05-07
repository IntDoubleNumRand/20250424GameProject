// GroundSpawner.cs
using Godot;
using System;

[Tool]
public partial class GroundSpawner : Node3D
{
	private const float GameAreaSize = 160f;
	private const int GridSize = 20;
	private static readonly float PathCellSize = GameAreaSize / GridSize;

	public Path Path { get; set; }

	public void SpawnGroundPatches()
	{
		var rng = new Random();
		int patchCount = rng.Next(30, 40);
		var areaRect = new Rect2(
			new Vector2(-GameAreaSize*0.5f, -GameAreaSize*0.5f),
			new Vector2(GameAreaSize, GameAreaSize)
		);

		// compute a padded bounding box around the path
		Rect2 paddedPathRect = new Rect2();
		if (Path != null)
		{
			float minX = float.MaxValue, maxX = float.MinValue, minZ = float.MaxValue, maxZ = float.MinValue;
			foreach (var cell in Path.AllPathCells)
			{
				float cx = (cell.X - GridSize*0.5f - 0.5f) * PathCellSize;
				float cz = (cell.Y - GridSize*0.5f - 0.5f) * PathCellSize;
				if (cx < minX) minX = cx;
				if (cx > maxX) maxX = cx;
				if (cz < minZ) minZ = cz;
				if (cz > maxZ) maxZ = cz;
			}
			var pathRect = new Rect2(
				new Vector2(minX - PathCellSize*0.5f, minZ - PathCellSize*0.5f),
				new Vector2(maxX - minX + PathCellSize, maxZ - minZ + PathCellSize)
			);
			paddedPathRect = pathRect.Grow(PathCellSize);
		}

		for (int i = 0; i < patchCount; i++)
		{
			int wCells = rng.Next(1, 3) * GridSize / 2;
			int dCells = rng.Next(1, 3) * GridSize / 2;
			float cellSize = PathCellSize;
			float patchW = wCells * cellSize;
			float patchD = dCells * cellSize;
			float halfW = patchW * 0.5f;
			float halfD = patchD * 0.5f;

			float x, z;
			bool bad;
			do
			{
				// allow center anywhere so patch may stick out
				x = (float)(rng.NextDouble() * (GameAreaSize + patchW)) - (GameAreaSize*0.5f + halfW);
				z = (float)(rng.NextDouble() * (GameAreaSize + patchD)) - (GameAreaSize*0.5f + halfD);

				var patchRect = new Rect2(new Vector2(x - halfW, z - halfD), new Vector2(patchW, patchD));

				bool overlapsPath = false;
				if (Path != null)
				{
					foreach (var cell in Path.AllPathCells)
					{
						float cx = (cell.X - GridSize*0.5f - 0.5f) * PathCellSize;
						float cz = (cell.Y - GridSize*0.5f - 0.5f) * PathCellSize;
						if (cx > x - halfW && cx < x + halfW && cz > z - halfD && cz < z + halfD)
						{
							overlapsPath = true;
							break;
						}
					}
				}

				bool surroundsPath = Path == null || patchRect.Intersects(paddedPathRect);
				bool overlapsGameArea = patchRect.Intersects(areaRect);

				bad = overlapsPath || !surroundsPath || !overlapsGameArea;
			}
			while (bad);

			float hillH = 1.5f;
			GroundBuilder.Create(this)
				.SetWidth(wCells)
				.SetDepth(dCells)
				.SetCellSize(cellSize)
				.SetEnableHill(true)
				.SetHillHeight(hillH)
				.SetFalloffExponent(0.6f)
				.SetLocation(new Vector3(x, 0, z))
				.Build();
		}
	}
}
