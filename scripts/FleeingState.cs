using Godot;
using System;

public class FleeingState : IAnimalState<Sheep>
{
	public void Enter(Sheep sheep) { }
	public void Exit(Sheep sheep) { }
	public void Update(Sheep sheep, double delta) { }
	public Vector3 GetMoveDirection(Sheep sheep)
	{
		if (sheep.Threat == null) return Vector3.Zero;
		var raw = sheep.GlobalPosition - sheep.Threat.GlobalPosition;
		return sheep.FlatDirection(raw);
	}
}
