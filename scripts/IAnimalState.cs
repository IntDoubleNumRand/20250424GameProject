using Godot;
using System;

public interface IAnimalState<T>
{
	void Enter(T animal);
	void Exit(T animal);
	void Update(T animal, double delta);
	Vector3 GetMoveDirection(T animal);
}
