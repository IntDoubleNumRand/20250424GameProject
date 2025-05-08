using Godot;
using System;
using System.Linq;

public partial class Sheep : CharacterBody3D
{
	private float _spriteFrameTimer = 0f;
	[Export] public float SpriteFrameInterval = 0.2f;
	
	[Export] public float MoveSpeed = 0.5f;
	[Export] public float AvoidDistance = 10f;
	[Export] public float AttractionRange = 10f;
	[Export] public float EatDistance = 0.1f;
	[Export] public float CheckInterval = 1f;

	[Export] public float MinFollowDist = 2f;
	[Export] public float MaxFollowDist = 6f;
	[Export] public float FollowJitterAmount = 0.3f;

	private Sprite3D _sprite;
	private Area3D _detectionArea;
	private Node3D _threat;
	private Node3D _plantTarget;
	private Node3D _sheepTarget;
	private Vector3 _wanderTarget;
	private Timer _scanTimer;
	private Random _rng = new();

	private enum SheepState { Fleeing, Eating, Following, Wandering }
	private SheepState _currentState = SheepState.Wandering;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite3D>("Sprite3D");
		_detectionArea = GetNode<Area3D>("Area3D");

		_detectionArea.BodyEntered += OnThreatEntered;
		_detectionArea.BodyExited  += OnThreatExited;

		_scanTimer = new Timer { WaitTime = CheckInterval, Autostart = true };
		_scanTimer.Timeout += ScanEnvironment;
		AddChild(_scanTimer);

		AddToGroup("Sheep");
		ChooseNewWanderTarget();
	}

	public override void _PhysicsProcess(double delta)
	{
		// Clean up any disposed references
		if (_threat != null && !Node.IsInstanceValid(_threat)) _threat = null;
		if (_plantTarget != null && !Node.IsInstanceValid(_plantTarget)) _plantTarget = null;
		if (_sheepTarget != null && !Node.IsInstanceValid(_sheepTarget)) _sheepTarget = null;

		UpdateState();
		Vector3 moveDir = GetMoveDirection();
		if (moveDir != Vector3.Zero)
		{
			Velocity = moveDir * MoveSpeed;
			MoveAndSlide();
		}

		_spriteFrameTimer += (float)delta;
		if (_spriteFrameTimer >= SpriteFrameInterval)
		{
			_spriteFrameTimer = 0f;
			UpdateSpriteFrame(moveDir);
		}
	}

	private void UpdateState()
	{
		if (_threat != null && GlobalPosition.DistanceTo(_threat.GlobalPosition) < AvoidDistance)
		{
			_currentState = SheepState.Fleeing;
		}
		else if (_plantTarget != null)
		{
			_currentState = SheepState.Eating;
		}
		else if (_sheepTarget != null)
		{
			_currentState = SheepState.Following;
		}
		else
		{
			_currentState = SheepState.Wandering;
		}
	}

	private Vector3 GetMoveDirection()
	{
		switch (_currentState)
		{
			case SheepState.Fleeing:
				return GetFleeDirection();

			case SheepState.Eating:
				return GetEatDirection();

			case SheepState.Following:
				return GetFollowDirection();

			case SheepState.Wandering:
			default:
				return GetWanderDirection();
		}
	}

	private Vector3 GetFleeDirection()
	{
		GD.Print("fleet");
		if (Velocity.Length() > 0.01f)
		{
			return FlatDirection(-Velocity.Normalized());
		}
		else
		{
			return FlatDirection(GlobalPosition - _threat.GlobalPosition);
		}
	}

	private Vector3 GetEatDirection()
	{
		GD.Print("Eating grass");
		if (_rng.NextDouble() < 1)
		{
			Vector3 dir = _plantTarget.GlobalPosition - GlobalPosition;
			if (GlobalPosition.DistanceTo(_plantTarget.GlobalPosition) < EatDistance)
			{
				EatPlant(_plantTarget);
				_plantTarget = null;
				ChooseNewWanderTarget();
			}
			return FlatDirection(dir);
		}
		else
		{
			_plantTarget = null;
			return Vector3.Zero;
		}
	}

	private Vector3 GetFollowDirection()
	{
		GD.Print("Following peer sheep");
		float dist = GlobalPosition.DistanceTo(_sheepTarget.GlobalPosition);
		if (dist < MinFollowDist)
		{
			return FlatDirection(GlobalPosition - _sheepTarget.GlobalPosition);
		}
		else if (dist < MaxFollowDist)
		{
			return FlatDirection(_sheepTarget.GlobalPosition - GlobalPosition);
		}
		else
		{
			GD.Print("Target too far wandering.");
			return GetWanderDirection();
		}
	}

	private Vector3 GetWanderDirection()
	{
		GD.Print("Wandering.");
		Vector3 dir = _wanderTarget - GlobalPosition;
		if (dir.Length() < 1f)
			ChooseNewWanderTarget();
		return FlatDirection(dir);
	}

	private Vector3 FlatDirection(Vector3 dir)
	{
		dir.Y = 0;
		return dir.Normalized();
	}

	private void ScanEnvironment()
	{
		_plantTarget = FindNearestPlant();
		_sheepTarget = FindNearbySheep();
	}

	private Node3D FindNearestPlant()
	{
		Node3D closest = null;
		float bestDist = AttractionRange;

		foreach (var plant in GetTree().GetNodesInGroup("Plant").OfType<Node3D>())
		{
			float dist = GlobalPosition.DistanceTo(plant.GlobalPosition);
			if (dist < bestDist)
			{
				bestDist = dist;
				closest = plant;
			}
		}

		return closest;
	}

	private Node3D FindNearbySheep()
	{
		Node3D best = null;
		float bestScore = float.MaxValue;

		foreach (var other in GetTree().GetNodesInGroup("Sheep").OfType<Node3D>())
		{
			if (other == this) continue;

			float dist = GlobalPosition.DistanceTo(other.GlobalPosition);
			if (dist >= MinFollowDist && dist <= MaxFollowDist)
			{
				float score = Mathf.Abs(dist - ((MinFollowDist + MaxFollowDist) / 2));
				if (score < bestScore)
				{
					bestScore = score;
					best = other;
				}
			}
		}

		return best;
	}

	private void EatPlant(Node3D plant)
	{
		GD.Print($"{Name} ate a plant at {plant.GlobalPosition}");
		plant.QueueFree();
	}

	private void OnThreatEntered(Node body)
	{
		if (body.IsInGroup("Threat") && body is Node3D node)
		{
			if (GlobalPosition.DistanceTo(node.GlobalPosition) < AvoidDistance)
				_threat = node;
		}
	}

	private void OnThreatExited(Node body)
	{
		if (body == _threat)
			_threat = null;
	}

	private void ChooseNewWanderTarget()
	{
		float range = 10f;
		_wanderTarget = GlobalPosition + new Vector3(
			((float)_rng.NextDouble() - 0.5f) * range,
			0,
			((float)_rng.NextDouble() - 0.5f) * range
		);
	}

	private void UpdateSpriteFrame(Vector3 dir)
	{
		if (dir == Vector3.Zero) return;
		float angle = Mathf.RadToDeg(Mathf.Atan2(dir.X, dir.Z));
		int frame = Mathf.Abs(angle) < 45 ? 0 :
					angle > 45 && angle < 135 ? 1 :
					Mathf.Abs(angle) > 135 ? 2 : 3;
		_sprite.Frame = frame;
	}
}
