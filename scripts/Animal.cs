using Godot;
using System;

public abstract partial class Animal : CharacterBody3D
{
	[Export] public float MoveSpeed = 0.5f;
	[Export] public float SpriteFrameInterval = 0.2f;

	protected Sprite3D Sprite3D;
	private float _spriteTimer;
	
	[Export] public float MaxHealth = 100f;
	public float Health { get; private set; }
	private ProgressBar _healthBar;
	
	
	public override void _Ready()
	{
		// Initialize health
		Health = MaxHealth;
		_healthBar = GetNode<ProgressBar>("HealthBar3D/SubViewport/ProgressBar");
		_healthBar.MaxValue = MaxHealth;
		_healthBar.Value    = Health;
	}

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
	
	public void TakeDamage(float amount)
	{
		Health = Mathf.Max(Health - amount, 0f);
		UpdateHealthBar();
		if (Health <= 0f) Die();
	}
	
	public void Heal(float amount)
	{
		Health = Mathf.Min(Health + amount, MaxHealth);
		UpdateHealthBar();
	}
	
	private void UpdateHealthBar()
	{
		if (_healthBar != null)
			_healthBar.Value = Health;
	}
	
	protected virtual void Die()
	{
		QueueFree();
	}

	protected abstract Vector3 GetMoveDirection(double delta);
}
