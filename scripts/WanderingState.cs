using Godot;
using System;

public class WanderingState : IAnimalState<Sheep>
{
	public void Enter(Sheep sheep) { GD.Print ( "Sheep wandering...");}
	public void Exit(Sheep sheep) { }

	public void Update(Sheep sheep, double delta)
	{
		var dir = sheep.WanderTarget - sheep.GlobalPosition;
		if (dir.Length() < 1f)
			sheep.ChooseNewWanderTarget();
	}

	public Vector3 GetMoveDirection(Sheep sheep)
	{
		var dir = sheep.WanderTarget - sheep.GlobalPosition;
		
		GD.Print ("Pos: " + sheep.GlobalPosition);
		return sheep.FlatDirection(dir);
	}
}
