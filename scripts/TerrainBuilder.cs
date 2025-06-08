using Godot;
using System;

public class TerrainBuilder
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

	private TerrainBuilder(Node3D parent) => _parent = parent;

	// factory entry point
	public static TerrainBuilder Create(Node3D parent) => new TerrainBuilder(parent);

	// setter methods
	public TerrainBuilder SetWidth(int value) { _width = value; return this; }
	public TerrainBuilder SetDepth(int value) { _depth = value; return this; }
	public TerrainBuilder SetCellSize(float value) { _cellSize = value; return this; }
	public TerrainBuilder SetEnableHill(bool value) { _enableHill = value; return this; }
	public TerrainBuilder SetHillHeight(float value) { _hillHeight = value; return this; }
	public TerrainBuilder SetPlateauT(float value) { _plateauT = value; return this; }
	public TerrainBuilder SetFalloffExponent(float value) { _falloffExponent = value; return this; }
	public TerrainBuilder SetColor(Color value) { _meshColor = value; return this; }
	public TerrainBuilder SetLocation(Vector3 value) { _location = value; return this; }

	// final build method
	public Terrain Build()
	{
		var terrain = new Terrain {
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
		terrain.AddChild(meshInst);
		terrain.AddChild(colShape);

		terrain.SetInternalParts(meshInst, colShape);
		
		terrain.GenerateTerrain();

		_parent.AddChild(terrain);
		return terrain;
	}
}
