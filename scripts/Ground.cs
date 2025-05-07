using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class Ground : StaticBody3D
{
	// Internal components
	private MeshInstance3D _meshInst;
	private CollisionShape3D _colShape;

	// avoid path
	private HashSet<Vector2> _pathCells;

	// Location in world
	[Export] public Vector3 Location { get; set; } = Vector3.Zero;
	
	// Grid & hill parameters
	[Export] public int Width { get; set; } = 10;
	[Export] public int Depth { get; set; } = 10;
	[Export] public float CellSize { get; set; } = 1f;
	[Export] public bool EnableHill { get; set; } = true;
	[Export] public float HillHeight { get; set; } = 2f;
	[Export] public float PlateauT { get; set; } = 0.8f;
	[Export] public float FalloffExponent { get; set; } = 0.5f;
	[Export] public Color MeshColor { get; set; } = new(0.2f, 0.4f, 0.1f);

	// Assign mesh and collider references
	public void SetInternalParts(MeshInstance3D meshInst, CollisionShape3D colShape)
	{
		_meshInst = meshInst;
		_colShape = colShape;
	}

	// Provide path cells for hill avoidance
	public void SetPathCells(HashSet<Vector2> pathCells)
	{
		_pathCells = pathCells;
	}

	// Main mesh generation function
	public void GenerateTerrain()
	{
		// Position node in world
		Position = Location;
		
		var st = new SurfaceTool();
		st.Begin(Mesh.PrimitiveType.Triangles);

		Vector2 center = new(Width * 0.5f, Depth * 0.5f);
		float maxDist = Math.Min(Width, Depth) * 0.5f;
		
		Vector3 offset = new Vector3(-Width * 0.5f * CellSize + Position.X, 0f, -Depth * 0.5f * CellSize + Position.Z);

		for (int x = 0; x < Width; x++)
		{
			for (int z = 0; z < Depth; z++)
			{
				// Calculate per-corner heights
				float h0 = (!EnableHill || ShouldCarve(x, z)) ? 0f : ComputeHeight(x, z, center, maxDist);
				float h1 = (!EnableHill || ShouldCarve(x+1, z)) ? 0f : ComputeHeight(x+1, z, center, maxDist);
				float h2 = (!EnableHill || ShouldCarve(x+1, z+1)) ? 0f : ComputeHeight(x+1, z+1, center, maxDist);
				float h3 = (!EnableHill || ShouldCarve(x, z+1)) ? 0f : ComputeHeight(x, z+1, center, maxDist);

				// Define the four corners with bump on Y
				Vector3 v0 = new Vector3(x * CellSize, h0, z * CellSize) + offset;
				Vector3 v1 = new Vector3((x+1) * CellSize, h1, z * CellSize) + offset;
				Vector3 v2 = new Vector3((x+1) * CellSize, h2, (z+1) * CellSize) + offset;
				Vector3 v3 = new Vector3(x * CellSize, h3, (z+1) * CellSize) + offset;

				// UVs tiled across the grid
				Vector2 uv0 = new Vector2((float)x / Width, (float)z / Depth);
				Vector2 uv1 = new Vector2((float)(x+1) / Width, (float)z / Depth);
				Vector2 uv2 = new Vector2((float)(x+1) / Width, (float)(z+1) / Depth);
				Vector2 uv3 = new Vector2((float)x / Width, (float)(z+1) / Depth);

				Vector3 normal = Vector3.Up;

				// First triangle (v0, v1, v2)
				st.SetNormal(normal); st.SetUV(uv0); st.AddVertex(v0);
				st.SetNormal(normal); st.SetUV(uv1); st.AddVertex(v1);
				st.SetNormal(normal); st.SetUV(uv2); st.AddVertex(v2);

				// Second triangle (v0, v2, v3)
				st.SetNormal(normal); st.SetUV(uv0); st.AddVertex(v0);
				st.SetNormal(normal); st.SetUV(uv2); st.AddVertex(v2);
				st.SetNormal(normal); st.SetUV(uv3); st.AddVertex(v3);
			}
		}

		// Commit the visual mesh
		var arrayMesh = st.Commit();
		_meshInst.Mesh = arrayMesh;

		// Build a concave collision shape from the mesh
		var triShape = arrayMesh.CreateTrimeshShape() as ConcavePolygonShape3D;
		_colShape.Shape = triShape;

		// Apply a simple material
		var mat = GD.Load<Material>("res://materials/Green1.tres");
		_meshInst.MaterialOverride = mat;
	}

	// Height falloff formula
	private float ComputeHeight(int xi, int zi, Vector2 center, float maxDist)
	{
		float dx = xi - center.X;
		float dz = zi - center.Y;
		float dist = Mathf.Sqrt(dx * dx + dz * dz);
		float t = Mathf.Clamp(1f - (dist / maxDist), 0f, 1f);

		if (t >= PlateauT)
			return HillHeight;

		float remap = t / PlateauT;
		float h = Mathf.Pow(remap, FalloffExponent);
		return HillHeight * h;
	}
	
	// Check if this grid corner is on the path
	private bool ShouldCarve(int xi, int zi)
	{
		return _pathCells != null && _pathCells.Contains(new Vector2(xi, zi));
	}
}
