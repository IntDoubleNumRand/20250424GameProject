using Godot;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class PathMeshBuilder : MeshInstance3D
{
	[Export] public NodePath CameraPath;
	[Export] public Material MudMaterial;
	[Export] public float Width = 1.0f;

	private Camera3D _camera;

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>(CameraPath);
		if (_camera == null || MudMaterial == null)
			return;

		var path = new PathOnScreen();
		var mapper = new ScreenToWorldMapper(_camera);
		var adapter = new WorldPathAdapter(path, mapper);
		var worldPoints = adapter.GetFullPath();
		
		var camPos = _camera.GlobalTransform.Origin;
		var camXZ = new Vector2(camPos.X, camPos.Z);
		
		var startPoint = worldPoints[0];
		var rest = worldPoints.Skip(1).ToList();
		var sortedWorldPoints = new List<Vector3> { startPoint };
		sortedWorldPoints.AddRange(rest);

		//GD.Print("Camera rotation (deg): ", _camera.RotationDegrees);
		//GD.Print("p: ");
		//foreach (var p in worldPoints)
			//GD.Print(p);

		var invXform = this.GlobalTransform.AffineInverse();
		var localPoints = sortedWorldPoints.Select(wp => invXform * wp).ToList();

		foreach (var wp in sortedWorldPoints)
		{
			var sphere = new SphereMesh
			{
				Radius = 0.3f,
				Height = 0.3f,
				RadialSegments = 8,
				Rings = 4
			};

			var marker = new MeshInstance3D
			{
				Mesh = sphere,
				MaterialOverride = new StandardMaterial3D
				{
					AlbedoColor = new Color(1, 0, 0),
					ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded
				}
			};
			AddChild(marker);
			marker.GlobalTransform = new Transform3D(Basis.Identity, wp);
		}

		var mesh = BuildPathMesh(localPoints, Width);
		var meshInstance = new MeshInstance3D
		{
			Mesh = mesh,
			Transform = Transform3D.Identity
		};
		AddChild(meshInstance);

		if (Mesh == null)
			GD.Print("Mesh was null.");
	}
	
	private Mesh BuildPathMesh(List<Vector3> points, float width)
	{
		if (points.Count < 2)
			return null;

		StandardMaterial3D matCopy = null;
		if (MudMaterial is StandardMaterial3D smd)
		{
			matCopy = (StandardMaterial3D)smd.Duplicate();
			matCopy.CullMode = BaseMaterial3D.CullModeEnum.Disabled;
		}

		var st = new SurfaceTool();
		st.Begin(Mesh.PrimitiveType.Triangles);
		st.SetMaterial(matCopy ?? MudMaterial);

		float half = width * 0.5f;

		for (int i = 0; i < points.Count - 1; i++)
		{
			var p1 = points[i];
			var p2 = points[i + 1];
			var dir1 = (p2 - p1).Normalized();
			var right1 = new Vector3(-dir1.Z, 0f, dir1.X).Normalized();

			// corners
			var leftA1 = p1 - right1 * half;
			var rightA1 = p1 + right1 * half;
			var leftB1 = p2 - right1 * half;
			var rightB1 = p2 + right1 * half;
			leftA1.Y = rightA1.Y = leftB1.Y = rightB1.Y = 0.001f;

			st.AddVertex(leftA1);
			st.AddVertex(leftB1);
			st.AddVertex(rightA1);

			st.AddVertex(rightA1);
			st.AddVertex(leftB1);
			st.AddVertex(rightB1);

			if (i < points.Count - 2)
			{
				var p3 = points[i + 2];
				var dir2 = (p3 - p2).Normalized();
				var right2 = new Vector3(-dir2.Z, 0f, dir2.X).Normalized();

				var leftA2 = p2 - right2 * half;
				var rightA2 = p2 + right2 * half;
				leftA2.Y = rightA2.Y = 0.001f;
				var p2Cap = new Vector3(p2.X, 0.001f, p2.Z);

				var cross = dir1.Cross(dir2);

				if (cross.Y > 0)
				{
					st.AddVertex(rightB1);
					st.AddVertex(p2Cap);
					st.AddVertex(rightA2);
				}
				else if (cross.Y < 0)
				{
					st.AddVertex(leftA2);
					st.AddVertex(p2Cap);
					st.AddVertex(leftB1);
				}
			}
		}
		st.GenerateNormals();
		st.Index();
		return st.Commit();
	}
}
