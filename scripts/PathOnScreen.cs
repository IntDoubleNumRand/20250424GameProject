using Godot;
using System;
using System.Collections.Generic;

public class PathOnScreen
{
	//public const float ScreenWidth = 1299f;
	public const float ScreenWidth = 1152f;
	
	public const float ScreenHeight = 648f;
	private const float StartY = 64.8f;
	private const float GoalY = ScreenHeight - (64.8f * 2);
	private const float MidpointTopY = StartY + 64.8f;
	private const float MidpointBottomY = GoalY - 64.8f;

	private Vector2 _start;
	private Vector2 _goal;
	private List<Vector2> _midPoints = new List<Vector2>();

	private static readonly Random _rng = new Random();

	public PathOnScreen()
	{
		GeneratePath();
	}
	
	private void GeneratePath()
	{
		float leftLimit = 0;
		float rightLimit = ScreenWidth;

		float middle = ScreenWidth / 2f;
		float buffer = ScreenWidth / 8f;

		bool startOnLeft = _rng.Next(0, 2) == 0;

		float a = RandomFloat(
			startOnLeft ? leftLimit : middle + buffer,
			startOnLeft ? middle - buffer : rightLimit
		);

		float b = RandomFloat(
			startOnLeft ? middle + buffer : leftLimit,
			startOnLeft ? rightLimit : middle - buffer
		);

		_start = new Vector2(a, StartY);
		_goal = new Vector2(b, GoalY);

		int midCount = _rng.Next(2, 5);
		_midPoints.Clear();

		for (int i = 0; i < midCount; i++)
		{
			float x = RandomFloat(Math.Min(a, b), Math.Max(a, b));
			float y = RandomFloat(MidpointTopY, MidpointBottomY);
			_midPoints.Add(new Vector2(x, y));
		}

		_midPoints.Sort((u, v) => u.Y.CompareTo(v.Y));

		GD.Print("Start: " + _start);
		GD.Print("Goal: " + _goal);
		GD.Print("Midpoints:");
		foreach (Vector2 mp in _midPoints)
		{
			GD.Print(mp);
		}
	}


	private float RandomFloat(float min, float max)
	{
		return (float)(_rng.NextDouble() * (max - min) + min);
	}

	public Vector2 Start => _start;
	public Vector2 Goal => _goal;
	public List<Vector2> MidPoints => new List<Vector2>(_midPoints);
}
