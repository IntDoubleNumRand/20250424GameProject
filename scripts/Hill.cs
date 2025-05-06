using Godot;
using System;

public class Hill
{
	public Vector2 GridPos { get; }
	public float   RawHeight { get; }
	public float   QuantizedHeight { get; }
	public bool    IsWalkable { get; }
	public Color   RenderColor { get; }

	public Hill(Vector2 gridPos, float rawH, float[] levels, Color[] colors, float walkableThresh)
	{
		GridPos   = gridPos;
		RawHeight = rawH;

		// determine bucket index
		int idx = 0;
		for (int i = 1; i < levels.Length; i++)
			if (rawH >= levels[i])
				idx = i;

		QuantizedHeight = levels[idx];
		RenderColor     = colors[Math.Min(idx, colors.Length - 1)];
		IsWalkable      = QuantizedHeight >= walkableThresh;
	}
}
