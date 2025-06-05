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
		var startPoint = worldPoints[0];
		var rest = worldPoints.Skip(1)
			.OrderBy(p => p.DistanceTo(camPos))
			.ToList();
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

		if (matCopy != null)
			st.SetMaterial(matCopy);
		else
			st.SetMaterial(MudMaterial);

		for (int i = 0; i < points.Count - 1; i++)
		{
			var p1 = points[i];
			var p2 = points[i + 1];
			var dir = (p2 - p1).Normalized();
			var right = new Vector3(-dir.Z, 0f, dir.X).Normalized();
			var half = width * 0.5f;

			var leftA = p1 - right * half;
			var rightA = p1 + right * half;
			var leftB = p2 - right * half;
			var rightB = p2 + right * half;
			
			leftA.Y = 0.1f;
			rightA.Y = 0.1f;
			leftB.Y = 0.1f;
			rightB.Y = 0.1f;

			st.AddVertex(leftA);
			st.AddVertex(leftB);
			st.AddVertex(rightA);

			st.AddVertex(rightA);
			st.AddVertex(leftB);
			st.AddVertex(rightB);
		}

		st.GenerateNormals();
		st.Index();
		return st.Commit();
	}
}
