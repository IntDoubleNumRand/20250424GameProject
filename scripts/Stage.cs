using Godot;
using System;
using System.Collections.Generic;

public partial class Stage : Node3D
{
	[Export] public NodePath CameraPath;

	public override void _Ready()
	{
		var camera = GetNode<Camera3D>(CameraPath);
		var path = new PathOnScreen();
		var mapper = new ScreenToWorldMapper(camera);
		var adapter = new WorldPathAdapter(path, mapper);

		ISpawnerStrategy terrainStrategy = new TerrainSpawnStrategy(
			camera.GlobalTransform.Origin,
			adapter
		);

		var terrainSpawner = new Lazy<TerrainSpawner>(() =>
		{
			var spawner = new TerrainSpawner(terrainStrategy) { Name = "TerrainSpawner" };
			AddChild(spawner);
			return spawner;
		});

		_ = terrainSpawner.Value;
		
		ISpawnerStrategy sheepStrategy = new SheepSpawnStrategy();
		var sheepSpawner = new Lazy<SheepSpawner>(() => { var s = new SheepSpawner(sheepStrategy, 0); AddChild(s); return s; });
		_ = sheepSpawner.Value;
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("reload_stage")){
			GD.Print("You pressed R");
			GetTree().ReloadCurrentScene();
		}
	}
}
