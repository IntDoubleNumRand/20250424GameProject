using Godot;
using System.Collections.Generic;

public enum PlantType { Yellow, Red, Purple, Grass }

public static class PlantFlyweightFactory
{
	private static readonly Dictionary<PlantType, PlantFlyweight> _flyweights;

	static PlantFlyweightFactory()
	{
		_flyweights = new Dictionary<PlantType, PlantFlyweight>
		{
			{ PlantType.Yellow, new PlantFlyweight(GD.Load<PackedScene>("res://scenes/FlowerYellow.tscn")) },
			{ PlantType.Red,    new PlantFlyweight(GD.Load<PackedScene>("res://scenes/FlowerRed.tscn")) },
			{ PlantType.Purple, new PlantFlyweight(GD.Load<PackedScene>("res://scenes/FlowerPurple.tscn")) },
			{ PlantType.Grass,  new PlantFlyweight(GD.Load<PackedScene>("res://scenes/Grass.tscn")) }
		};
	}

	public static PlantFlyweight Get(PlantType type) => _flyweights[type];
}
