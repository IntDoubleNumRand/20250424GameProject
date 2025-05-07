// GameArea.cs
using Godot;
using System;

[Tool]
public partial class GameArea : StaticBody3D
{
	public override void _Ready()
	{
		// Create mesh instance
		var meshInstance = new MeshInstance3D();
		AddChild(meshInstance);

		// Create a plane mesh
		var plane = new PlaneMesh
		{
			Size = new Vector2(160, 160)
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
			Size = new Vector3(160, 0.1f, 160)
		};
		collision.Shape = shape;
		collision.Position = new Vector3(0, -0.05f, 0);

		// Build and draw the path
		var path = new Path();
		path.Draw(this);

		// Spawn patches around it
		var spawner = new GroundSpawner();
		AddChild(spawner);
		spawner.Path = path;
		spawner.SpawnGroundPatches();
	}
}
