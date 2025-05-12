using Godot;
using System;

public class GroundBuilder
{
	private readonly Node3D _parent;

	private int _width = 10;
	private int _depth = 10;
	private float _cellSize = 1f;
	private bool _enableHill = true;
	private float _hillHeight = 2f;
	private float _plateauT = 0.8f;
	private float _falloffExponent = 0.5f;
	private Color _meshColor = new(0.2f, 0.4f, 0.1f);
	private Vector3 _location = Vector3.Zero;

	private GroundBuilder(Node3D parent) => _parent = parent;

	// factory entry point
	public static GroundBuilder Create(Node3D parent) => new GroundBuilder(parent);

	// setter methods
	public GroundBuilder SetWidth(int value) { _width = value; return this; }
	public GroundBuilder SetDepth(int value) { _depth = value; return this; }
	public GroundBuilder SetCellSize(float value) { _cellSize = value; return this; }
	public GroundBuilder SetEnableHill(bool value) { _enableHill = value; return this; }
	public GroundBuilder SetHillHeight(float value) { _hillHeight = value; return this; }
	public GroundBuilder SetPlateauT(float value) { _plateauT = value; return this; }
	public GroundBuilder SetFalloffExponent(float value) { _falloffExponent = value; return this; }
	public GroundBuilder SetColor(Color value) { _meshColor = value; return this; }
	public GroundBuilder SetLocation(Vector3 value) { _location = value; return this; }

	// final build method
	public Ground Build()
	{
		var ground = new Ground {
			Width = _width,
			Depth = _depth,
			CellSize = _cellSize,
			EnableHill = _enableHill,
			HillHeight = _hillHeight,
			PlateauT = _plateauT,
			FalloffExponent = _falloffExponent,
			MeshColor = _meshColor,
			Location = _location
		};

		var meshInst = new MeshInstance3D();
		var colShape = new CollisionShape3D();
		ground.AddChild(meshInst);
		ground.AddChild(colShape);

		ground.SetInternalParts(meshInst, colShape);
		
		ground.GenerateTerrain();

		_parent.AddChild(ground);
		return ground;
	}
}
