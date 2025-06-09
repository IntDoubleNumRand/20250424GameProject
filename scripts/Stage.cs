using Godot;
using System;

public partial class Stage : Node3D
{
	[Export] public NodePath CameraPath;
	[Export] public int SheepCount = 1;
	[Export] public int WolfCount = 1;
	[Export] public int PlantCount = 30;

	private Lazy<TerrainSpawner> _terrainSpawner;
	private Lazy<SheepSpawner> _sheepSpawner;
	private Lazy<WolfSpawner> _wolfSpawner;
	private Lazy<PlantSpawner> _plantSpawner;

	public override void _Ready()
	{
		var settings = (Godot.Collections.Dictionary<string, int>)GetTree().Root.GetMeta("GameSettings");
		SheepCount = settings["sheep"];
		WolfCount = settings["wolf"];
		SheepManager.Instance?.InitializeSheepCount(SheepCount);
		SetupTerrainSpawner();
		SetupSheepSpawner();
		SetupWolfSpawner();
		SetupPlantSpawner();
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

	private void SetupPlantSpawner()
	{
		var camera = GetNode<Camera3D>(CameraPath);
		var path = new PathOnScreen();
		var mapper = new ScreenToWorldMapper(camera);
		var adapter = new WorldPathAdapter(path, mapper);
		var terrainStrategy = new TerrainSpawnStrategy(camera.GlobalTransform.Origin, adapter);
		var terrainPositions = terrainStrategy.GenerateSpawnPositions();

		ISpawnerStrategy plantStrategy = new PlantSpawnStrategy(
			camera.GlobalTransform.Origin,
			terrainPositions,
			minCount: PlantCount,
			maxCount: PlantCount
		);

		_plantSpawner = new Lazy<PlantSpawner>(() =>
		{
			var spawner = new PlantSpawner(plantStrategy) { Name = "PlantSpawner" };
			AddChild(spawner);
			return spawner;
		});
		_ = _plantSpawner.Value;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("reload_stage"))
		{
			GD.Print("You pressed R");

			foreach (var sheep in GetTree().GetNodesInGroup("Sheep"))
			{
				if (sheep is Node node)
				{
					node.QueueFree();
				}
			}

			SheepManager.Instance?.ResetGameState();
			GetTree().ReloadCurrentScene();
		}
	}
}
