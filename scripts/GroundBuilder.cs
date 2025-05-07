// GroundBuilder.cs
using Godot;
using System;

public class GroundBuilder
{
	private readonly Node3D parent;
	private int width = 15;
	private int depth = 15;
	private float cellSize = 1f;
	private bool enableHill = true;
	private float hillHeight = 2f;
	private float falloffExponent = 0.5f;
	private Vector3 location = Vector3.Zero;

	private GroundBuilder(Node3D parent) => this.parent = parent;
	public static GroundBuilder Create(Node3D parent) => new GroundBuilder(parent);

	public GroundBuilder SetWidth(int value) { width = value; return this; }
	public GroundBuilder SetDepth(int value) { depth = value; return this; }
	public GroundBuilder SetCellSize(float value) { cellSize = value; return this; }
	public GroundBuilder SetEnableHill(bool value) { enableHill = value; return this; }
	public GroundBuilder SetHillHeight(float value) { hillHeight = value; return this; }
	public GroundBuilder SetFalloffExponent(float value) { falloffExponent = value; return this; }
	public GroundBuilder SetLocation(Vector3 value) { location = value; return this; }

	public Ground Build()
	{
		var ground = new Ground
		{
			Width = width,
			Depth = depth,
			CellSize = cellSize,
			EnableHill = enableHill,
			HillHeight = hillHeight,
			FalloffExponent = falloffExponent,
			Location = location
		};

		var meshInst = new MeshInstance3D();
		var colShape = new CollisionShape3D();
		ground.AddChild(meshInst);
		ground.AddChild(colShape);

		ground.SetInternalParts(meshInst, colShape);
		ground.GenerateTerrain();

		parent.AddChild(ground);
		return ground;
	}
}
