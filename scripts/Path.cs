// Path.cs
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Path
{
	private const int GridSize = 20;
	private const float GameAreaSize = 160f;
	private static readonly float WorldCell = GameAreaSize / GridSize;

	private Vector2 _start;
	private Vector2 _goal;
	private List<Vector2> _midPoints;
	private HashSet<Vector2> _pathCells;
	private static Random _rng = new Random();

	public Path()
	{
		GeneratePath();
	}

	private void GeneratePath()
	{
		int a = _rng.Next(1, GridSize + 1);
		int b = a <= GridSize/2
			? _rng.Next(GridSize/2 + 1, GridSize + 1)
			: _rng.Next(1, GridSize/2 + 1);
		while (Math.Abs(a - b) < 2)
			b = a <= GridSize/2
				? _rng.Next(GridSize/2 + 1, GridSize + 1)
				: _rng.Next(1, GridSize/2 + 1);

		_start = new Vector2(a, 1);
		_goal  = new Vector2(b, GridSize);

		int diff    = Math.Abs(a - b);
		int minMid  = Math.Max(1, Math.Min(diff, 4));
		int count   = _rng.Next(minMid, 5);

		_midPoints = new List<Vector2>();
		for (int i = 0; i < count; i++)
		{
			int x = _rng.Next(Math.Min(a, b), Math.Max(a, b) + 1);
			int y = _rng.Next(3, GridSize);
			_midPoints.Add(new Vector2(x, y));
		}

		BuildPath();
	}

	private void BuildPath()
	{
		_pathCells = new HashSet<Vector2>();
		var pts = new List<Vector2> { _start }
			.Concat(_midPoints.OrderBy(p => p.Y))
			.Append(_goal)
			.ToList();

		for (int i = 0; i < pts.Count - 1; i++)
			AddLine(pts[i], pts[i+1]);
	}

	private void AddLine(Vector2 from, Vector2 to)
	{
		var pos = from;
		var dx = Math.Sign(to.X - from.X);
		var dy = Math.Sign(to.Y - from.Y);
		while (pos != to)
		{
			_pathCells.Add(pos);
			if (pos.X != to.X) pos.X += dx;
			if (pos.Y != to.Y) pos.Y += dy;
		}
		_pathCells.Add(to);
	}

	// draws each cell as a flat box
	public void Draw(Node parent)
	{
		var mat = GD.Load<StandardMaterial3D>("res://materials/Dirt.tres");

		foreach (var cell in _pathCells)
		{
			var inst = new MeshInstance3D();
			inst.Mesh = new BoxMesh { Size = new Vector3(WorldCell, 0.1f, WorldCell) };
			inst.MaterialOverride = mat;

			float x = (cell.X - GridSize * 0.5f - 0.5f) * WorldCell;
			float z = (cell.Y - GridSize * 0.5f - 0.5f) * WorldCell;
			inst.Position = new Vector3(x, 0.05f, z);

			parent.AddChild(inst);
		}
	}

	public HashSet<Vector2> AllPathCells => _pathCells;
}
