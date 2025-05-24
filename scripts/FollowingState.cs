using Godot;
using System;

public class FollowingState : IAnimalState<Sheep>
{
	public void Enter(Sheep sheep) { }
	public void Exit(Sheep sheep) { }

	public void Update(Sheep sheep, double delta)
	{
		if (sheep.SheepTarget == null)
			sheep.ChangeState(sheep.WanderingState);
	}

	public Vector3 GetMoveDirection(Sheep sheep)
	{
		if (sheep.SheepTarget == null) return Vector3.Zero;

		float dist = sheep.GlobalPosition.DistanceTo(sheep.SheepTarget.GlobalPosition);
		if (dist < sheep.MinFollowDist)
			return sheep.FlatDirection(sheep.GlobalPosition - sheep.SheepTarget.GlobalPosition);
		if (dist < sheep.MaxFollowDist)
			return sheep.FlatDirection(sheep.SheepTarget.GlobalPosition - sheep.GlobalPosition);

		return Vector3.Zero;
	}
}
