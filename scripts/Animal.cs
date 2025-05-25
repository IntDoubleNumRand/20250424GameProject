using Godot;
using System;

public abstract partial class Animal : CharacterBody3D
{
	[Export] public float MoveSpeed = 0.5f;
	[Export] public float SpriteFrameInterval = 0.2f;
	[Export] public float MaxHealth = 100f;

	public float Health { get; private set; }
	private ProgressBar _healthBar;

	public override void _Ready()
	{
		Health = MaxHealth;
		var hbNode = GetNodeOrNull<Node>("HealthBar3D/SubViewport/ProgressBar");
		if (hbNode is ProgressBar pb)
		{
			_healthBar = pb;
			_healthBar.MaxValue = MaxHealth;
			_healthBar.Value = Health;
			GD.Print($"{Name} found ProgressBar and set to {Health}/{MaxHealth}");
		}
		else
		{
			GD.PrintErr($"{Name} could not find ProgressBar at 'HealthBar3D/SubViewport/ProgressBar'");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
	}

	public virtual void TakeDamage(float amount)
	{
		GD.Print($"{Name} TakeDamage({amount}) called. Health before: {Health}");
		Health = Mathf.Max(Health - amount, 0f);
		UpdateHealthBar();
		if (Health <= 0f)
		{
			Die();
		}
	}

	public virtual void Heal(float amount)
	{
		Health = Mathf.Min(Health + amount, MaxHealth);
		UpdateHealthBar();
	}

	protected void UpdateHealthBar()
	{
		if (_healthBar != null)
		{
			_healthBar.Value = Health;
				
			GD.Print($"{Name} health bar updated to {Health}");
			GD.Print($"{Name} health bar actually updated to {_healthBar.Value}");
		}
		else
		{
			GD.PrintErr($"{Name} health bar is NULL in UpdateHealthBar");
		}
	}

	protected virtual void Die()
	{
		GD.Print($"{Name} dies.");
		QueueFree();
	}

	protected abstract Vector3 GetMoveDirection(double delta);
}
