using Godot;
using System;

[Tool]
public partial class GreenGround : StaticBody3D
{
	public override void _Ready()
	{
		// Create mesh instance
		var meshInstance = new MeshInstance3D();
		AddChild(meshInstance);

		// Create a plane mesh
		var plane = new PlaneMesh
		{
			Size = new Vector2(100, 100)
		};
		meshInstance.Mesh = plane;

		// Create green material
		var mat = new StandardMaterial3D
		{
			AlbedoColor = new Color(0.2f, 0.8f, 0.2f)
		};
		meshInstance.MaterialOverride = mat;

		// Create collision shape
		var collision = new CollisionShape3D();
		AddChild(collision);

		var shape = new BoxShape3D
		{
			Size = new Vector3(100, 0.1f, 100)
		};
		collision.Shape = shape;

		// Slightly offset collider down so it's under the plane
		collision.Position = new Vector3(0, -0.05f, 0);
		
		var spawner = new GroundSpawner();
		AddChild(spawner);
		spawner.SpawnGroundPatches();
	}
}
