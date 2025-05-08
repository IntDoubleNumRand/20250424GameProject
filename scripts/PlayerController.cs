using Godot;
using System;

public partial class PlayerController : Node3D
{
	[Export] public float WeaponLength = 2f;
	[Export] public NodePath CharacterPath = "CharacterVisual";
	[Export] public NodePath WeaponPath = "WeaponMesh";

	private Sprite3D _character;
	private Node3D _weapon;
	private Camera3D _camera;

	public override void _Ready()
	{		
		_character = GetNodeOrNull<Sprite3D>(CharacterPath);
		if (_character == null)
			_character = GetNodeOrNull<Sprite3D>("CharacterVisual");

		_weapon = GetNodeOrNull<Node3D>(WeaponPath);
		if (_weapon == null)
			_weapon = GetNodeOrNull<Node3D>("WeaponMesh");

		_camera = GetViewport().GetCamera3D();
		if (_camera == null)
			GD.PushError("Camera3D not found!");
			
			
	}

	public override void _Input(InputEvent @event)
	{
		GD.Print("Input action 1");
		if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Right && mb.Pressed)
		{
			GD.Print("Input action 2");
			TryPickUpPlant();
		}
	}

	public override void _Process(double delta)
	{
		if (_camera == null) return;

		// Project mouse to ground plane (Y = -1.0)
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector3 from = _camera.ProjectRayOrigin(mousePos);
		Vector3 dir = _camera.ProjectRayNormal(mousePos);

		Plane ground = new Plane(Vector3.Up, -1.0f);
		if (!IntersectRayWithPlane(from, dir, ground, out var target)) return;

		// Move player so weapon tip touches the mouse location
		Vector3 toTarget = (target - GlobalPosition).Normalized();
		GlobalPosition = target - toTarget * WeaponLength;

		// Rotate player and weapon to face target
		LookAt(target, Vector3.Up);
		_weapon.GlobalPosition = GlobalPosition + toTarget * (WeaponLength * 0.5f);
		_weapon.LookAt(target, Vector3.Up);

		// Change face sprite frame
		UpdateFace(toTarget);
	}

	private void UpdateFace(Vector3 dir)
	{
		if (_character == null) return;

		float angle = Mathf.RadToDeg(Mathf.Atan2(dir.X, dir.Z));
		int frame;

		if (Mathf.Abs(angle) < 45) frame = 0; // front
		else if (angle > 45 && angle < 135) frame = 1; // left
		else if (Mathf.Abs(angle) > 135) frame = 2; // back
		else frame = 3;  // right

		_character.Frame = frame;
	}
	
	private void TryPickUpPlant()
	{
		var mousePos = GetViewport().GetMousePosition();
		var from = _camera.ProjectRayOrigin(mousePos);
		var to = from + _camera.ProjectRayNormal(mousePos) * 1000f;

		var space = GetWorld3D().DirectSpaceState;
		var query = new PhysicsRayQueryParameters3D
		{
			From = from,
			To = to,
			CollisionMask = uint.MaxValue
		};

		var result = space.IntersectRay(query);

		if (result.Count > 0)
		{
			GD.Print("Found collider");
			var collider = result["collider"].As<Node>();
			GD.Print($"Collided with {result}");
			GD.Print($"Collided with {collider}");
			if (collider != null && collider.IsInGroup("Plant"))
			{
				GD.Print($"Picked up: {collider.Name}");
				collider.QueueFree();
			}
		}
	}

	private bool IntersectRayWithPlane(Vector3 from, Vector3 dir, Plane plane, out Vector3 hit)
	{
		float denom = plane.Normal.Dot(dir);
		if (Mathf.Abs(denom) < 0.0001f)
		{
			hit = Vector3.Zero;
			return false;
		}

		float t = -(plane.Normal.Dot(from) + plane.D) / denom;
		if (t < 0)
		{
			hit = Vector3.Zero;
			return false;
		}

		hit = from + dir * t;
		return true;
	}
}
