using Godot;
using System;
using System.Linq;

public partial class Wolf : AnimalContext<Wolf>
{
	[Export] public float AvoidDistance = 8f;
	[Export] public float HuntRange = 15f;
	[Export] public float EatDistance = 1.5f;
	[Export] public float CheckInterval = 1f;

	internal Node3D  Threat { get; private set; }
	internal Node3D  PreyTarget { get; private set; }
	internal Vector3 WanderTarget { get; private set; }

	internal readonly IAnimalState<Wolf> FleeingState = new WolfFleeingState();
	internal readonly IAnimalState<Wolf> HuntingState = new WolfHuntingState();
	internal readonly IAnimalState<Wolf> EatingState = new WolfEatingState();
	internal readonly IAnimalState<Wolf> WanderingState = new WolfWanderingState();

	private Sprite3D _sprite;
	private Timer _scanTimer;
	private Random _rng = new();

	public override void _Ready()
	{
		base._Ready();

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
		if (Threat != null && !Godot.GodotObject.IsInstanceValid(Threat))
			Threat = null;
		if (PreyTarget != null && !Godot.GodotObject.IsInstanceValid(PreyTarget))
			PreyTarget = null;

		float dThreat = Threat?.GlobalPosition.DistanceTo(GlobalPosition) ?? float.MaxValue;
		float dPrey = PreyTarget?.GlobalPosition.DistanceTo(GlobalPosition) ?? float.MaxValue;

		// Transition logic
		if (Threat != null && dThreat < AvoidDistance)
			ChangeState(FleeingState);
		else if (PreyTarget != null && dPrey <= EatDistance)
			ChangeState(EatingState);
		else if (PreyTarget != null)
			ChangeState(HuntingState);
		else
			ChangeState(WanderingState);
	}
	
	public override void PostPhysicsUpdate(Vector3 moveDir, double delta)
	{
		base.PostPhysicsUpdate(moveDir, delta);
		if (moveDir == Vector3.Zero) return;
		float angle = Mathf.RadToDeg(Mathf.Atan2(moveDir.X, moveDir.Z));
		int frame = Mathf.Abs(angle) < 45 ? 2
				  : angle > 45 && angle < 135 ? 1
				  : Mathf.Abs(angle) > 135 ? 0: 3;
		_sprite.Frame = frame;
	}
	
	private void ScanEnvironment()
	{
		PreyTarget = FindNearest("Sheep", HuntRange);
		Threat = FindNearest("Weapon", AvoidDistance);
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
	
	protected override void Die()
	{
		GD.Print("Wolf died");
		base.Die();
	}
	
	private void InitializeWanderTarget()
	{
		float r = 12f;
		WanderTarget = GlobalPosition + new Vector3(
			(_rng.NextSingle() - 0.5f)*r, 0, (_rng.NextSingle() - 0.5f)*r
		);
	}
	
	public Vector3 FlatDirection(Vector3 v)
	{
		v.Y = 0;
		return v.Normalized();
	}
	
	private class WolfFleeingState : IAnimalState<Wolf>
	{
		public void Enter(Wolf w) => GD.Print("Wolf: Fleeing");
		public void Exit(Wolf w) { }
		public void Update(Wolf w, double dt) { }
		public Vector3 GetMoveDirection(Wolf w) =>
			w.Threat == null ? Vector3.Zero
				: w.FlatDirection(w.GlobalPosition - w.Threat.GlobalPosition);
	}
	
	private class WolfHuntingState : IAnimalState<Wolf>
	{
		public void Enter(Wolf w) => GD.Print("Wolf: Hunting");
		public void Exit(Wolf w) { }
		public void Update(Wolf w, double dt) { }
		public Vector3 GetMoveDirection(Wolf w) =>
			w.PreyTarget == null ? Vector3.Zero
				: w.FlatDirection(w.PreyTarget.GlobalPosition - w.GlobalPosition);
	}
	
	private class WolfEatingState : IAnimalState<Wolf>
	{
		private float _eatTimer = 0f;
		
		public void Enter(Wolf w)
		{
			GD.Print("Wolf Eating");
			_eatTimer = 0f;
		}
		
		public void Exit(Wolf w) { }
		
		public void Update(Wolf w, double dt)
		{
			if (!(w.PreyTarget is Sheep sheep) || !Godot.GodotObject.IsInstanceValid(sheep))
			{
				w.ChangeState(w.WanderingState);
				return;
			}
			
			_eatTimer += (float)dt;
			if (_eatTimer >= 1f)
			{
				GD.Print($"Wolf bites sheep {sheep.Name} → -20 HP");
				sheep.TakeDamage(20);
				_eatTimer = 0f;
			}
		}
		
		public Vector3 GetMoveDirection(Wolf w) => Vector3.Zero;
	}
	
	private class WolfWanderingState : IAnimalState<Wolf>
	{
		public void Enter(Wolf w) => GD.Print("Wolf: Wandering");
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
