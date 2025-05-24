using Godot;
using System.Collections.Generic;

[Tool]
public partial class PathMeshBuilder : MeshInstance3D
{
	[Export] public NodePath CameraPath;
	[Export] public Material MudMaterial;
	[Export] public float Width = 2f;

	private Camera3D _camera;

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>(CameraPath);
		if (_camera == null || MudMaterial == null)
			return;

		var path = new PathOnScreen();
		var mapper = new ScreenToWorldMapper(_camera);
		var adapter = new WorldPathAdapter(path, mapper);
		var points = adapter.GetFullPath();
		
		GD.Print("Camera rotation (deg): ", _camera.RotationDegrees);
		GD.Print("p: ");
		foreach (var p in points)
			GD.Print(p);
			
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		GD.Print(screenSize);
		GD.Print(screenSize);
			

			
		foreach(var point in points)
		{
			var sphere = new SphereMesh();
			sphere.Radius = 0.3f;
			sphere.Height = 0.3f;
			sphere.RadialSegments = 8;
			sphere.Rings = 4;

			var marker = new MeshInstance3D();
			marker.Mesh = sphere;

			var mat = new StandardMaterial3D
			{
				AlbedoColor = new Color(1, 0, 0),
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded
			};
			marker.MaterialOverride = mat;
			AddChild(marker);
			marker.GlobalTransform = new Transform3D(Basis.Identity, point);
			GD.Print($"Global point: {point}");
		}

		var mesh = BuildPathMesh(points, 2.0f);
		var meshInstance = new MeshInstance3D();
		meshInstance.Mesh = mesh;
		meshInstance.GlobalTransform = Transform3D.Identity;
		AddChild(meshInstance);

		if (Mesh == null)
			GD.Print("Mesh was null.");
	}

	private Mesh BuildPathMesh(List<Vector3> points, float width)
	{
		if (points.Count < 2)
			return null;

		var st = new SurfaceTool();
		st.Begin(Mesh.PrimitiveType.Triangles);
		st.SetMaterial(MudMaterial);

		for (int i = 0; i < points.Count - 1; i++)
		{
			var p1 = points[i];
			var p2 = points[i + 1];
			var dir = (p2 - p1).Normalized();
			var right = new Vector3(-dir.Z, 0f, dir.X).Normalized();

			var leftA = p1 - right * (width * 0.5f);
			var rightA = p1 + right * (width * 0.5f);
			var leftB = p2 - right * (width * 0.5f);
			var rightB = p2 + right * (width * 0.5f);

			st.AddVertex(leftA);
			st.AddVertex(rightA);
			st.AddVertex(leftB);

			st.AddVertex(rightA);
			st.AddVertex(rightB);
			st.AddVertex(leftB);
		}

		st.Index();
		return st.Commit();
	}
}
