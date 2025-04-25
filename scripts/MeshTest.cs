// MeshTest.cs
using Godot;
using System;

[Tool]
public partial class MeshTest : MeshInstance3D
{
	// Backing fields
	private int width = 10;
	private int depth = 10;
	private float cellSize = 1f;

	[Export]
	public int Width {
		get => width;
		set {
			width = value;
			if (Engine.IsEditorHint())
				GenerateMesh();
		}
	}

	[Export]
	public int Depth {
		get => depth;
		set {
			depth = value;
			if (Engine.IsEditorHint())
				GenerateMesh();
		}
	}

	[Export]
	public float CellSize {
		get => cellSize;
		set {
			cellSize = value;
			if (Engine.IsEditorHint())
				GenerateMesh();
		}
	}

	public override void _Ready()
	{
		// Generate once when the scene loads (including in the editor)
		if (Engine.IsEditorHint())
			GenerateMesh();
	}

	private void GenerateMesh()
	{
		var st = new SurfaceTool();
		st.Begin(Mesh.PrimitiveType.Triangles);

		for (int x = 0; x < width; x++)
		{
			for (int z = 0; z < depth; z++)
			{
				// Corners of the quad
				Vector3 v0 = new(x * cellSize, 0, z * cellSize);
				Vector3 v1 = new((x + 1) * cellSize, 0, z * cellSize);
				Vector3 v2 = new((x + 1) * cellSize, 0, (z + 1) * cellSize);
				Vector3 v3 = new(x * cellSize, 0, (z + 1) * cellSize);

				// Tiled UVs
				Vector2 uv0 = new((float)x / width, (float)z / depth);
				Vector2 uv1 = new((float)(x + 1) / width, (float)z / depth);
				Vector2 uv2 = new((float)(x + 1) / width, (float)(z + 1) / depth);
				Vector2 uv3 = new((float)x / width, (float)(z + 1) / depth);

				// Flat normal
				Vector3 normal = Vector3.Up;

				// First triangle
				st.SetNormal(normal);
				st.SetUV(uv0);
				st.AddVertex(v0);

				st.SetNormal(normal);
				st.SetUV(uv1);
				st.AddVertex(v1);

				st.SetNormal(normal);
				st.SetUV(uv2);
				st.AddVertex(v2);

				// Second triangle
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

		// Commit and assign
		Mesh = st.Commit();
	}
}
