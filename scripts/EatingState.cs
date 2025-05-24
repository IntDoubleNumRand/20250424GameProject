using Godot;
using System;

public class EatingState : IAnimalState<Sheep>
{
	public void Enter(Sheep sheep)  { }
	public void Exit(Sheep sheep) { }

	public void Update(Sheep sheep, double delta)
	{
		if (sheep.PlantTarget == null)
		{
			sheep.ChangeState(sheep.WanderingState);
			return;
		}

		float dist = sheep.GlobalPosition.DistanceTo(sheep.PlantTarget.GlobalPosition);
		if (dist < sheep.EatDistance)
		{
			sheep.EatPlant(sheep.PlantTarget);
			sheep.ScanEnvironment();
			sheep.ChangeState(sheep.WanderingState);
		}
	}

	public Vector3 GetMoveDirection(Sheep sheep)
	{
		if (sheep.PlantTarget == null) return Vector3.Zero;
		var raw = sheep.PlantTarget.GlobalPosition - sheep.GlobalPosition;
		return sheep.FlatDirection(raw);
	}
}
