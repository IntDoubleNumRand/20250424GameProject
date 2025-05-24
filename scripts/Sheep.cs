using Godot;
using System;
using System.Linq;

public partial class Sheep : AnimalContext<Sheep>
{
	[Export] public float MaxSpeed         = 0.5f;
	[Export] public float AvoidDistance    = 10f;
	[Export] public float AttractionRange  = 10f;
	[Export] public float EatDistance      = 0.1f;
	[Export] public float MinFollowDist    = 2f;
	[Export] public float MaxFollowDist    = 6f;
	[Export] public float CheckInterval    = 1f;
	[Export] public float SpriteFrameInterval = 0.2f;

	// States
	internal readonly IAnimalState<Sheep> FleeingState   = new FleeingState();
	internal readonly IAnimalState<Sheep> EatingState    = new EatingState();
	internal readonly IAnimalState<Sheep> FollowingState = new FollowingState();
	internal readonly IAnimalState<Sheep> WanderingState = new WanderingState();

	// Current targets
	internal Node3D Threat       { get; private set; }
	internal Node3D PlantTarget  { get; private set; }
	internal Node3D SheepTarget  { get; private set; }
	internal Vector3 WanderTarget { get; private set; }

	// Sprite & timer
	private Sprite3D _sprite;
	private Timer   _scanTimer;
	private Random  _rng = new();

	// Provide MoveSpeed to base context
	public override float MoveSpeed => MaxSpeed;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite3D>("Sprite3D");

		// periodic scanning
		_scanTimer = new Timer { WaitTime = CheckInterval, Autostart = true };
		_scanTimer.Timeout += ScanEnvironment;
		AddChild(_scanTimer);

		AddToGroup("Sheep");
		ChooseNewWanderTarget();
		ChangeState(WanderingState);
	}

	public override void EvaluateStateTransitions()
	{
		float toThreat = Threat?.GlobalPosition.DistanceTo(GlobalPosition) ?? float.MaxValue;

		if (Threat != null && toThreat < AvoidDistance)
			ChangeState(FleeingState);
		else if (PlantTarget != null)
			ChangeState(EatingState);
		else if (SheepTarget != null)
			ChangeState(FollowingState);
		else
			ChangeState(WanderingState);
	}

	// after movement, update sprite
	public override void PostPhysicsUpdate(Vector3 dir, double delta)
	{
		base.PostPhysicsUpdate(dir, delta);

		if (dir == Vector3.Zero) return;
		float angle = Mathf.RadToDeg(Mathf.Atan2(dir.X, dir.Z));
		int frame = Mathf.Abs(angle) < 45 ? 0
				  : angle > 45 && angle < 135 ? 3
				  : Mathf.Abs(angle) > 135 ? 2 : 1;
		_sprite.Frame = frame;
	}

	// find plants, sheep, threats
	internal void ScanEnvironment()
	{
		PlantTarget = FindNearestGroup("Plant", AttractionRange);
		SheepTarget = FindNearestSheep();
		Threat      = FindNearestGroup("Threat", AvoidDistance);
	}

	private Node3D FindNearestGroup(string group, float maxRange)
	{
		Node3D best = null; float bestD = maxRange;
		foreach (Node3D n in GetTree().GetNodesInGroup(group).OfType<Node3D>())
		{
			float d = GlobalPosition.DistanceTo(n.GlobalPosition);
			if (d < bestD) { bestD = d; best = n; }
		}
		return best;
	}

	private Node3D FindNearestSheep()
	{
		Node3D best = null; float bestScore = float.MaxValue;
		foreach (Sheep other in GetTree().GetNodesInGroup("Sheep").OfType<Sheep>())
		{
			if (other == this) continue;
			float d = GlobalPosition.DistanceTo(other.GlobalPosition);
			if (d >= MinFollowDist && d <= MaxFollowDist)
			{
				float score = MathF.Abs(d - (MinFollowDist + MaxFollowDist)/2);
				if (score < bestScore) { bestScore = score; best = other; }
			}
		}
		return best;
	}

	public void EatPlant(Node3D plant)
	{
		GD.Print($"{Name} ate plant at {plant.GlobalPosition}");
		plant.QueueFree();
	}

	internal void ChooseNewWanderTarget()
	{
		float r = 10f;
		WanderTarget = GlobalPosition + new Vector3(
			(_rng.NextSingle() - 0.5f)*r, 0,
			(_rng.NextSingle() - 0.5f)*r
		);
	}

	public Vector3 FlatDirection(Vector3 v)
	{
		v.Y = 0;
		return v.Normalized();
	}
}
