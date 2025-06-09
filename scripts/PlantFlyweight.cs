using Godot;

public class PlantFlyweight
{
	private readonly PackedScene _scene;

	public PlantFlyweight(PackedScene scene)
	{
		_scene = scene;
	}

	public Node3D Instantiate(Vector3 position, float scale)
	{
		var plant = _scene.Instantiate<Node3D>();
		plant.Position = position;
		plant.Scale = new Vector3(scale, scale, scale);
		plant.AddToGroup("Plant");
		return plant;
	}
}
