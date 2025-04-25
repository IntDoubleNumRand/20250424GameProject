/// MeshTest.cs
using Godot;
using System;

[Tool] // run in editor
public partial class MeshTest : MeshInstance3D
{
	// Grid dimensions
	private int width = 10;
	private int depth = 10;
	private float cellSize = 1f;

	// Hill controls
	private bool  enableHill = true;
	private float hillHeight = 2f;

	[Export]
	public int Width {
		get => width;
		set {
			width = value;
			if (Engine.IsEditorHint()) GenerateMesh();
		}
	}

	[Export]
	public int Depth {
		get => depth;
		set {
			depth = value;
			if (Engine.IsEditorHint()) GenerateMesh();
		}
	}

	[Export]
	public float CellSize {
		get => cellSize;
		set {
			cellSize = value;
			if (Engine.IsEditorHint()) GenerateMesh();
		}
	}

	[Export]
	public bool EnableHill {
		get => enableHill;
		set {
			enableHill = value;
			if (Engine.IsEditorHint()) GenerateMesh();
		}
	}

	[Export]
	public float HillHeight {
		get => hillHeight;
		set {
			hillHeight = value;
			if (Engine.IsEditorHint()) GenerateMesh();
		}
	}
	
	[Export]
	public Color MeshColor { get; set; } = new Color(0.2f, 0.4f, 0.1f);
	
	public override void _Ready()
	{
		// Build once on load (including in the editor)
		if (Engine.IsEditorHint())
			GenerateMesh();
	}

	private void GenerateMesh()
	{
		var st = new SurfaceTool();
		st.Begin(Mesh.PrimitiveType.Triangles);

		// Center coordinates for hill math
		Vector2 center = new(width * 0.5f, depth * 0.5f);
		float maxDist = Math.Min(width, depth) * 0.5f;

		for (int x = 0; x < width; x++)
		{
			for (int z = 0; z < depth; z++)
			{
				// Calculate per-corner heights
				float h0 = enableHill ? ComputeHeight(x,   z,   center, maxDist) : 0f;
				float h1 = enableHill ? ComputeHeight(x+1, z,   center, maxDist) : 0f;
				float h2 = enableHill ? ComputeHeight(x+1, z+1, center, maxDist) : 0f;
				float h3 = enableHill ? ComputeHeight(x,   z+1, center, maxDist) : 0f;

				// Define the four corners with bump on Y
				Vector3 v0 = new(x   * cellSize, h0, z   * cellSize);
				Vector3 v1 = new((x+1) * cellSize, h1, z   * cellSize);
				Vector3 v2 = new((x+1) * cellSize, h2, (z+1) * cellSize);
				Vector3 v3 = new(x   * cellSize, h3, (z+1) * cellSize);

				// UVs tiled across the grid
				Vector2 uv0 = new((float)x   / width, (float)z   / depth);
				Vector2 uv1 = new((float)(x+1) / width, (float)z   / depth);
				Vector2 uv2 = new((float)(x+1) / width, (float)(z+1) / depth);
				Vector2 uv3 = new((float)x   / width, (float)(z+1) / depth);

				Vector3 normal = Vector3.Up;

				// First triangle (v0, v1, v2)
				st.SetNormal(normal);
				st.SetUV(uv0);
				st.AddVertex(v0);

				st.SetNormal(normal);
				st.SetUV(uv1);
				st.AddVertex(v1);

				st.SetNormal(normal);
				st.SetUV(uv2);
				st.AddVertex(v2);

				// Second triangle (v0, v2, v3)
				st.SetNormal(normal);
				st.SetUV(uv0);
				st.AddVertex(v0);

				st.SetNormal(normal);
				st.SetUV(uv2);
				st.AddVertex(v2);

				st.SetNormal(normal);
				st.SetUV(uv3);
				st.AddVertex(v3);
			}
		}

		// Commit mesh and assign
		var arrayMesh = st.Commit();
		Mesh = arrayMesh;

		// Create & assign a simple material using your exported color
		var mat = new StandardMaterial3D();
		mat.AlbedoColor = MeshColor;
		// If you want a little specular highlight:
		mat.Metallic = 0.1f;
		mat.Roughness = 0.8f;

		MaterialOverride = mat;
	}

	[Export] public float PlateauT = 0.8f;    // 80% radius flat top
	[Export] public float FalloffExponent = 0.5f;  // sqrt‐style slope

	private float ComputeHeight(int xi, int zi, Vector2 center, float maxDist)
	{
		// 1) normalized 0 at edge → 1 at center
		float dx = xi - center.X;
		float dz = zi - center.Y;
		float dist = Mathf.Sqrt(dx*dx + dz*dz);
		float t = Mathf.Clamp(1f - (dist / maxDist), 0f, 1f);

		// 2) plateau if inside threshold
		if (t >= PlateauT)
			return hillHeight;

		// 3) else remap and pow
		float remap = t / PlateauT;                          // now 0→1
		float h = Mathf.Pow(remap, FalloffExponent);          // gentle curve
		return hillHeight * h;
	}

}
