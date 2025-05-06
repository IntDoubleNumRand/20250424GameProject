using Godot;
using System;

[Tool]
public partial class MeshTest : MeshInstance3D
{
	[Export] public int     Width             = 10;
	[Export] public int     Depth             = 10;
	[Export] public float   CellSize          = 1f;
	[Export] public bool    EnableHill        = true;
	[Export] public float   HillHeight        = 2f;
	[Export] public float[] Levels            = new float[]{ 0f, 0.5f, 1f };
	[Export] public Color[] LevelColors       = new Color[]{ new Color(0.2f,0.5f,0.2f), new Color(0.6f,0.9f,0.6f), new Color(0.8f,0.8f,0.5f) };
	[Export] public float   WalkableThreshold = 0.5f;

	private Hill[,] hills;

	public override void _Ready()
	{
		if (Engine.IsEditorHint())
			GenerateMesh();
	}

	private void GenerateMesh()
	{
		// instantiate Hill objects
		hills = new Hill[Width + 1, Depth + 1];
		Vector2 center = new Vector2(Width * 0.5f, Depth * 0.5f);
		float maxDist = Math.Min(Width, Depth) * 0.5f;

		for (int x = 0; x <= Width; x++)
		for (int z = 0; z <= Depth; z++)
		{
			float rawH = EnableHill
				? ComputeHeight(x, z, center, maxDist) * HillHeight
				: 0f;

			hills[x, z] = new Hill(
				new Vector2(x, z),
				rawH,
				Levels,
				LevelColors,
				WalkableThreshold
			);
		}

		// build the mesh
		var st = new SurfaceTool();
		st.Begin(Mesh.PrimitiveType.Triangles);

		for (int x = 0; x < Width; x++)
		for (int z = 0; z < Depth; z++)
		{
			Hill h0 = hills[x,   z];
			Hill h1 = hills[x+1, z];
			Hill h2 = hills[x+1, z+1];
			Hill h3 = hills[x,   z+1];

			// first triangle
			AddVertex(st, x,   z,   h0);
			AddVertex(st, x+1, z,   h1);
			AddVertex(st, x+1, z+1, h2);

			// second triangle
			AddVertex(st, x,   z,   h0);
			AddVertex(st, x+1, z+1, h2);
			AddVertex(st, x,   z+1, h3);
		}

		Mesh = st.Commit();
		ApplyMaterial();
		BakeCollision();
	}

	private void AddVertex(SurfaceTool st, int x, int z, Hill hill)
	{
		st.SetColor(hill.RenderColor);
		st.SetNormal(Vector3.Up);
		st.SetUV(new Vector2((float)x / Width, (float)z / Depth));
		st.AddVertex(new Vector3(x * CellSize, hill.QuantizedHeight, z * CellSize));
	}

	private void ApplyMaterial()
	{
		var mat = new StandardMaterial3D();
		mat.VertexColorUseAsAlbedo = true;
		MaterialOverride = mat;
	}

	private void BakeCollision()
	{
		var parent = GetParent() as StaticBody3D;
		if (parent == null)
		{
			GD.PrintErr("MeshTest must be a child of StaticBody3D for collision to work.");
			return;
		}

		var col = parent.GetNodeOrNull<CollisionShape3D>("CollisionShape3D");
		if (col == null)
		{
			col = new CollisionShape3D { Name = "CollisionShape3D", Owner = Owner };
			parent.AddChild(col);
		}
		col.Shape = Mesh.CreateTrimeshShape();
	}

	private float ComputeHeight(int xi, int zi, Vector2 center, float maxDist)
	{
		float dx   = xi - center.X;
		float dz   = zi - center.Y;
		float dist = Mathf.Sqrt(dx * dx + dz * dz);
		float t    = Mathf.Clamp(1f - (dist / maxDist), 0f, 1f);
		return t;
	}
}
