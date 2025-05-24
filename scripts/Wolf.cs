using Godot;
using System;
using System.Linq;

public partial class Wolf : AnimalContext<Wolf>
{
	[Export] public float MaxSpeed            = 0.8f;
	[Export] public float AvoidDistance       = 8f;
	[Export] public float HuntRange           = 15f;
	[Export] public float EatDistance         = 0.5f;
	[Export] public float CheckInterval       = 1f;
	[Export] public float SpriteFrameInterval = 0.2f;

	internal Node3D  Threat        { get; private set; }
	internal Node3D  PreyTarget    { get; private set; }
	internal Vector3 WanderTarget  { get; private set; }

	internal readonly IAnimalState<Wolf> FleeingState   = new WolfFleeingState();
	internal readonly IAnimalState<Wolf> HuntingState   = new WolfHuntingState();
	internal readonly IAnimalState<Wolf> EatingState    = new WolfEatingState();
	internal readonly IAnimalState<Wolf> WanderingState = new WolfWanderingState();

	private Sprite3D _sprite;
	private Timer    _scanTimer;
	private Random   _rng = new();

	public override float MoveSpeed => MaxSpeed;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite3D>("Sprite3D");

		_scanTimer = new Timer { WaitTime = CheckInterval, Autostart = true };
		_scanTimer.Timeout += ScanEnvironment;
		AddChild(_scanTimer);

		AddToGroup("Wolf");
		InitializeWanderTarget();
		ChangeState(WanderingState);
	}

	public override void EvaluateStateTransitions()
	{
		float dThreat = Threat?.GlobalPosition.DistanceTo(GlobalPosition) ?? float.MaxValue;
		float dPrey   = PreyTarget?.GlobalPosition.DistanceTo(GlobalPosition) ?? float.MaxValue;

		if (Threat != null && dThreat < AvoidDistance)
			ChangeState(FleeingState);
		else if (PreyTarget != null && dPrey <= EatDistance)
			ChangeState(EatingState);
		else if (PreyTarget != null)
			ChangeState(HuntingState);
		else
			ChangeState(WanderingState);
	}

	private void ScanEnvironment()
	{
		// hunt sheep
		PreyTarget = FindNearest("Sheep", HuntRange);

		// Avoid thret
		Threat = FindNearest("Weapon", AvoidDistance);
	}

	public override void PostPhysicsUpdate(Vector3 moveDir, double delta)
	{
		if (moveDir == Vector3.Zero) return;
		float angle = Mathf.RadToDeg(Mathf.Atan2(moveDir.X, moveDir.Z));
		int frame = Mathf.Abs(angle) < 45 ? 0
				  : angle > 45 && angle < 135 ? 3
				  : Mathf.Abs(angle) > 135 ? 2: 1;
		_sprite.Frame = frame;
	}

	private Node3D FindNearest(string group, float range)
	{
		Node3D best = null;
		float bestD = range;
		foreach (Node3D n in GetTree().GetNodesInGroup(group).OfType<Node3D>())
		{
			float d = GlobalPosition.DistanceTo(n.GlobalPosition);
			if (d < bestD) { bestD = d; best = n; }
		}
		return best;
	}

	public void EatPrey(Node3D prey)
	{
		GD.Print($"Wolf ate prey at {prey.GlobalPosition}");
		prey.QueueFree();
	}

	private void InitializeWanderTarget()
	{
		float r = 12f;
		WanderTarget = GlobalPosition + new Vector3(
			(_rng.NextSingle() - 0.5f) * r,
			0,
			(_rng.NextSingle() - 0.5f) * r
		);
	}

	public Vector3 FlatDirection(Vector3 v)
	{
		v.Y = 0;
		return v.Normalized();
	}

	private class WolfFleeingState : IAnimalState<Wolf>
	{
		public void Enter(Wolf w) { GD.Print("wolf Fleeing"); }
		public void Exit(Wolf w)  { }
		public void Update(Wolf w, double dt) { }
		public Vector3 GetMoveDirection(Wolf w) =>
			w.Threat == null
				? Vector3.Zero
				: w.FlatDirection(w.GlobalPosition - w.Threat.GlobalPosition);
	}

	private class WolfHuntingState : IAnimalState<Wolf>
	{
		public void Enter(Wolf w) {GD.Print($"wolf Hunting");}
		public void Exit(Wolf w) { }
		public void Update(Wolf w, double dt) { }
		public Vector3 GetMoveDirection(Wolf w) =>
			w.PreyTarget == null
				? Vector3.Zero
				: w.FlatDirection(w.PreyTarget.GlobalPosition - w.GlobalPosition);
	}

	private class WolfEatingState : IAnimalState<Wolf>
	{
		public void Enter(Wolf w)
		{
			GD.Print($"Wolf eating");
			if (w.PreyTarget != null) w.EatPrey(w.PreyTarget);
		}
		public void Exit(Wolf w) { }
		public void Update(Wolf w, double dt)
		{
			w.ScanEnvironment();
			w.ChangeState(w.WanderingState);
		}
		public Vector3 GetMoveDirection(Wolf w) => Vector3.Zero;
	}

	private class WolfWanderingState : IAnimalState<Wolf>
	{
		public void Enter(Wolf w){GD.Print("Wolf: Enter Wandering");}
		public void Exit(Wolf w) { }
		public void Update(Wolf w, double dt)
		{
			var diff = w.WanderTarget - w.GlobalPosition;
			if (diff.Length() < 1f) w.InitializeWanderTarget();
		}
		public Vector3 GetMoveDirection(Wolf w) =>
			w.FlatDirection(w.WanderTarget - w.GlobalPosition);
	}
}
