// Path.cs
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Path
{
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
		int a = _rng.Next(1, 5); // 1 to 4 inclusive
		int b;

		// rule: |a-b| >= 2, a and b on the opposite side
		if (a <= 4)
			b = _rng.Next(5, 9); // 5 to 8
		else
			b = _rng.Next(1, 5); // 1 to 4

		while (Math.Abs(a - b) < 2)
			b = (a <= 4) ? _rng.Next(5, 9) : _rng.Next(1, 5);

		_start = new Vector2(a, 1);
		_goal = new Vector2(b, 8);

		int minMid = Math.Max(1, Math.Min(4, Math.Abs(a - b)));
		int midCount = _rng.Next(minMid, 5);

		_midPoints = new List<Vector2>();
		for (int i = 0; i < midCount; i++)
		{
			int x = _rng.Next(Math.Min(a, b), Math.Max(a, b) + 1);
			int y = _rng.Next(3, 7); // [3,6]
			_midPoints.Add(new Vector2(x, y));
		}

		BuildPath();
	}

	private void BuildPath()
	{
		_pathCells = new HashSet<Vector2>();
		List<Vector2> allPoints = new List<Vector2> { _start };
		allPoints.AddRange(_midPoints.OrderBy(p => p.Y)); // top to bottom
		allPoints.Add(_goal);

		for (int i = 0; i < allPoints.Count - 1; i++)
		{
			AddLineCells(allPoints[i], allPoints[i + 1]);
		}
	}

	private void AddLineCells(Vector2 from, Vector2 to)
	{
		int dx = (int)Math.Sign(to.X - from.X);
		int dy = (int)Math.Sign(to.Y - from.Y);
		Vector2 pos = from;

		while (pos != to)
		{
			_pathCells.Add(pos);
			if (pos.X != to.X) pos.X += dx;
			if (pos.Y != to.Y) pos.Y += dy;
		}
		_pathCells.Add(to);
	}

	// Getters
	public Vector2 Start => _start;
	public Vector2 Goal => _goal;
	public List<Vector2> MidPoints => _midPoints;
	public HashSet<Vector2> AllPathCells => _pathCells;
}
