using Godot;
using System;

public partial class Sheep : CharacterBody3D
{
	[Export] public float MoveSpeed = 2f;
	[Export] public float AvoidDistance = 5f;

	private Sprite3D _sprite;
	private Area3D _detectionArea;
	private Vector3 _targetPosition;
	private Vector3 _avoidDirection = Vector3.Zero;
	private Node3D _threat;

	private Random _rng = new();

	public override void _Ready()
	{
		// Automatically get child nodes by name
		_sprite = GetNode<Sprite3D>("Sprite3D");
		_detectionArea = GetNode<Area3D>("Area3D");

		_detectionArea.BodyEntered += OnThreatEntered;
		_detectionArea.BodyExited += OnThreatExited;

		ChooseNewWanderTarget();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 moveDir;

		if (_threat != null && GlobalPosition.DistanceTo(_threat.GlobalPosition) < AvoidDistance)
		{
			// Run away
			_avoidDirection = (GlobalPosition - _threat.GlobalPosition).Normalized();
			moveDir = _avoidDirection;
		}
		else
		{
			// Wander
			moveDir = (_targetPosition - GlobalPosition).Normalized();
			if (GlobalPosition.DistanceTo(_targetPosition) < 1f)
				ChooseNewWanderTarget();
		}

		// Apply movement
		Velocity = moveDir * MoveSpeed;
		MoveAndSlide();

		// Update visual direction
		UpdateSpriteFrame(moveDir);
	}

	private void ChooseNewWanderTarget()
	{
		float range = 10f;
		_targetPosition = GlobalPosition + new Vector3(
			(float)(_rng.NextDouble() - 0.5) * range,
			0,
			(float)(_rng.NextDouble() - 0.5) * range
		);
	}

	private void OnThreatEntered(Node body)
	{
		if (body.Name == "WeaponMesh") // match the name exactly
			_threat = body as Node3D;
	}

	private void OnThreatExited(Node body)
	{
		if (body == _threat)
			_threat = null;
	}

	private void UpdateSpriteFrame(Vector3 dir)
	{
		if (dir == Vector3.Zero) return;

		float angle = Mathf.RadToDeg(Mathf.Atan2(dir.X, dir.Z));
		int frame;

		if (Mathf.Abs(angle) < 45)           frame = 0; // Front
		else if (angle > 45 && angle < 135)  frame = 1; // Left
		else if (Mathf.Abs(angle) > 135)     frame = 2; // Back
		else                                 frame = 3; // Right

		_sprite.Frame = frame;
	}
}
