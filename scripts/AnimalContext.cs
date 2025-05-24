using Godot;
using System;

public abstract partial class AnimalContext<T> : CharacterBody3D where T : AnimalContext<T>
{
	protected IAnimalState<T> _currentState;

	public void ChangeState(IAnimalState<T> newState)
	{
		if (_currentState == newState) return;
		_currentState?.Exit((T)this);
		_currentState = newState;
		_currentState.Enter((T)this);
	}

	public override void _PhysicsProcess(double delta)
	{
		EvaluateStateTransitions();
		_currentState.Update((T)this, delta);

		Vector3 dir = _currentState.GetMoveDirection((T)this);
		if (dir != Vector3.Zero)
		{
			Velocity = dir * MoveSpeed;
			MoveAndSlide();
		}

		PostPhysicsUpdate(dir, delta);
	}

	public abstract void EvaluateStateTransitions();

	public abstract float MoveSpeed { get; }

	public virtual void PostPhysicsUpdate(Vector3 moveDir, double delta) { }
}
