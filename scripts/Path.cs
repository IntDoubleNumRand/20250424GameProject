using Godot;
using System;
using System.Collections.Generic;

public class Path
{
	public const float ScreenWidth = 1299f;
	public const float ScreenHeight = 648f;
	private const float StartY = 64.8f;
	private const float GoalY = ScreenHeight - (64.8f * 2);
	private const float MidpointTopY = StartY + 64.8f;
	private const float MidpointBottomY = GoalY - 64.8f;

	private Vector2 _start;
	private Vector2 _goal;
	private List<Vector2> _midPoints = new();

	private static readonly Random _rng = new();

	public Path()
	{
		GeneratePath();
	}

	private void GeneratePath()
	{
		float b = RandomFloat(0, ScreenWidth);
		_start = new Vector2(b, StartY);

		float a = RandomFloat(0, ScreenWidth);
		_goal = new Vector2(a, GoalY);

		int midCount = _rng.Next(2, 5);
		_midPoints.Clear();

		for (int i = 0; i < midCount; i++)
		{
			float x = RandomFloat(0, ScreenWidth);
			float y = RandomFloat(MidpointTopY, MidpointBottomY);
			_midPoints.Add(new Vector2(x, y));
		}

		_midPoints.Sort((aVec, bVec) => aVec.Y.CompareTo(bVec.Y));
	}

	private float RandomFloat(float min, float max)
	{
		return (float)(_rng.NextDouble() * (max - min) + min);
	}

	public Vector2 Start => _start;
	public Vector2 Goal => _goal;
	public List<Vector2> MidPoints => new List<Vector2>(_midPoints);

	public List<Vector2> GetFullPath()
	{
		var fullPath = new List<Vector2> { _start };
		fullPath.AddRange(_midPoints);
		fullPath.Add(_goal);
		return fullPath;
	}

	public void Regenerate()
	{
		GeneratePath();
	}
}
