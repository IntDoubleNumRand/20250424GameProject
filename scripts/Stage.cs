using Godot;
using System;

public partial class Stage : Node3D
{
	[Export] public NodePath CameraPath;
	[Export] public int SheepCount = 1;
	[Export] public int WolfCount = 1;

	private Lazy<TerrainSpawner> _terrainSpawner;
	private Lazy<SheepSpawner> _sheepSpawner;
	private Lazy<WolfSpawner> _wolfSpawner;

	public override void _Ready()
	{
		SetupTerrainSpawner();
		SetupSheepSpawner();
		SetupWolfSpawner();
	}

	private void SetupTerrainSpawner()
	{
		var camera = GetNode<Camera3D>(CameraPath);
		var path = new PathOnScreen();
		var mapper = new ScreenToWorldMapper(camera);
		var adapter = new WorldPathAdapter(path, mapper);
		ISpawnerStrategy terrainStrategy = new TerrainSpawnStrategy(camera.GlobalTransform.Origin, adapter);
		_terrainSpawner = new Lazy<TerrainSpawner>(() =>
		{
			var spawner = new TerrainSpawner(terrainStrategy) { Name = "TerrainSpawner" };
			AddChild(spawner);
			return spawner;
		});
		_ = _terrainSpawner.Value;
	}

	private void SetupSheepSpawner()
	{
		ISpawnerStrategy sheepStrategy = new SheepSpawnStrategy(SheepCount);
		_sheepSpawner = new Lazy<SheepSpawner>(() =>
		{
			var spawner = new SheepSpawner(sheepStrategy, 0) { Name = "SheepSpawner" };
			AddChild(spawner);
			return spawner;
		});
		_ = _sheepSpawner.Value;
	}

	private void SetupWolfSpawner()
	{
		ISpawnerStrategy wolfStrategy = new WolfSpawnStrategy(WolfCount);
		_wolfSpawner = new Lazy<WolfSpawner>(() =>
		{
			var spawner = new WolfSpawner(wolfStrategy, 0) { Name = "WolfSpawner" };
			AddChild(spawner);
			return spawner;
		});
		_ = _wolfSpawner.Value;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("reload_stage"))
		{
			GD.Print("You pressed R");
			GetTree().ReloadCurrentScene();
		}
	}
}
