// Ground.cs
using Godot;
using System;

[Tool]
public partial class Ground : StaticBody3D
{
	private MeshInstance3D _meshInst;
	private CollisionShape3D _colShape;

	[Export]
	public Vector3 Location { get; set; } = Vector3.Zero;
	[Export]
	public int Width { get; set; } = 10;
	[Export]
	public int Depth { get; set; } = 10;
	[Export]
	public float CellSize { get; set; } = 1f;
	[Export]
	public bool EnableHill { get; set; } = true;
	[Export]
	public float HillHeight { get; set; } = 2f;
	[Export]
	public float FalloffExponent { get; set; } = 0.5f;

	// Assign mesh and collider references
	public void SetInternalParts(MeshInstance3D meshInst, CollisionShape3D colShape)
	{
		_meshInst = meshInst;
		_colShape = colShape;
	}

	// Main mesh generation function
	public void GenerateTerrain()
	{
		Position = Location;
		var st = new SurfaceTool();
		st.Begin(Mesh.PrimitiveType.Triangles);

		Vector2 center = new Vector2(Width * 0.5f, Depth * 0.5f);
		float halfW = Width * 0.5f;
		float halfD = Depth * 0.5f;
		Vector3 offset = new Vector3(-Width * CellSize * 0.5f + Position.X, 0f, -Depth * CellSize * 0.5f + Position.Z);

		for (int x = 0; x < Width; x++)
		{
			for (int z = 0; z < Depth; z++)
			{
				float h0 = EnableHill ? ComputeHeight(x,   z,   center, halfW, halfD) : 0f;
				float h1 = EnableHill ? ComputeHeight(x+1, z,   center, halfW, halfD) : 0f;
				float h2 = EnableHill ? ComputeHeight(x+1, z+1, center, halfW, halfD) : 0f;
				float h3 = EnableHill ? ComputeHeight(x,   z+1, center, halfW, halfD) : 0f;

				Vector3 v0 = new Vector3(x  * CellSize, h0, z  * CellSize) + offset;
				Vector3 v1 = new Vector3((x+1)* CellSize, h1, z  * CellSize) + offset;
				Vector3 v2 = new Vector3((x+1)* CellSize, h2, (z+1)* CellSize) + offset;
				Vector3 v3 = new Vector3(x  * CellSize, h3, (z+1)* CellSize) + offset;

				Vector2 uv0 = new Vector2((float)x   / Width, (float)z   / Depth);
				Vector2 uv1 = new Vector2((float)(x+1)/ Width, (float)z   / Depth);
				Vector2 uv2 = new Vector2((float)(x+1)/ Width, (float)(z+1)/ Depth);
				Vector2 uv3 = new Vector2((float)x   / Width, (float)(z+1)/ Depth);

				Vector3 normal = Vector3.Up;
				st.SetNormal(normal); st.SetUV(uv0); st.AddVertex(v0);
				st.SetNormal(normal); st.SetUV(uv1); st.AddVertex(v1);
				st.SetNormal(normal); st.SetUV(uv2); st.AddVertex(v2);

				st.SetNormal(normal); st.SetUV(uv0); st.AddVertex(v0);
				st.SetNormal(normal); st.SetUV(uv2); st.AddVertex(v2);
				st.SetNormal(normal); st.SetUV(uv3); st.AddVertex(v3);
			}
		}

		var mesh = st.Commit();
		_meshInst.Mesh = mesh;
		_colShape.Shape = mesh.CreateTrimeshShape() as ConcavePolygonShape3D;

		// Random green variant
		var rng = new Random();
		int choice = rng.Next(1, 11);
		var mat = GD.Load<StandardMaterial3D>($"res://materials/Green{choice}.tres");
		_meshInst.MaterialOverride = mat;
	}

	private float ComputeHeight(int xi, int zi, Vector2 center, float halfW, float halfD)
	{
		float dx = xi - center.X;
		float dz = zi - center.Y;
		float ndx = dx / halfW;
		float ndz = dz / halfD;
		float distNorm = Mathf.Sqrt(ndx * ndx + ndz * ndz);
		float t = Mathf.Clamp(1f - distNorm, 0f, 1f);
		return Mathf.Pow(t, FalloffExponent) * HillHeight;
	}
}
