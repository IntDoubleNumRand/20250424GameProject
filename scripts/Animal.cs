using Godot;
using System;

public abstract partial class Animal : CharacterBody3D
{
	[Export] public float MoveSpeed = 0.5f;
	[Export] public float SpriteFrameInterval = 0.2f;

	protected Sprite3D Sprite3D;
	private float _spriteTimer;

	public override void _PhysicsProcess(double delta)
	{
		// Delegate movement to derived class
		Vector3 dir = GetMoveDirection(delta);

		// Move
		if (dir != Vector3.Zero)
		{
			Velocity = dir * MoveSpeed;
			MoveAndSlide();
		}

		// Animate
		_spriteTimer += (float)delta;
		if (_spriteTimer >= SpriteFrameInterval)
		{
			_spriteTimer = 0f;
			if (dir != Vector3.Zero)
			{
				float angle = Mathf.RadToDeg(Mathf.Atan2(dir.X, dir.Z));
				int frame = Mathf.Abs(angle) < 45f ? 0 :
							(angle > 45f && angle < 135f) ? 1 :
							(Mathf.Abs(angle) > 135f) ? 2 : 3;
				Sprite3D.Frame = frame;
			}
		}
	}

	protected abstract Vector3 GetMoveDirection(double delta);
}
