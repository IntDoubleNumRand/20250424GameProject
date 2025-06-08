using Godot;
using System;

public partial class PlayerController : Node3D
{
	[Export] public float WeaponLength = 2f;
	[Export] public NodePath CharacterPath = "Character";
	[Export] public NodePath WeaponPath = "Weapon";

	private Sprite3D _character;
	private Node3D _weapon;
	private Camera3D _camera;
	private PlayerFacade _player;

	public override void _Ready()
	{		
		_character = GetNodeOrNull<Sprite3D>(CharacterPath);
		if (_character == null)
			_character = GetNodeOrNull<Sprite3D>("Character");

		_weapon = GetNodeOrNull<Node3D>(WeaponPath);
		if (_weapon == null)
			_weapon = GetNodeOrNull<Node3D>("Weapon");

		_camera = GetViewport().GetCamera3D();
		if (_camera == null)
			GD.PushError("Camera3D not found!");
			
		_player = new PlayerFacade(_camera, GetViewport(), GetWorld3D());
	}

	public override void _Input(InputEvent @event)
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		//GD.Print("Input action 1: " + Input.MouseMode);
		if (@event is InputEventMouseButton mb && mb.Pressed)
		{
			//GD.Print("Input action 2: " + Input.MouseMode);
			Vector2 clickPos = mb.Position;
			
			if (mb.ButtonIndex == MouseButton.Right)
			{
				GD.Print("Mouse is right clicked once.");
				Wolf wolf = _player.TryHitWolf(clickPos);
				if (wolf != null)
				{
					GD.Print("Hit a Wolf! Calling TakeDamage(30)");
					wolf.TakeDamage(30);
					return;
				}
			}
			if (mb.ButtonIndex == MouseButton.Left)
			{
				GD.Print("Mouse is left clicked once.");
			}
			_player.TryPickUpPlant(clickPos);
		}
	}

	public override void _Process(double delta)
	{
		if (_camera == null) return;
		
		if (Input.IsActionJustPressed("save_sheep"))
		{
			SheepManager.Instance?.SaveSheep();
			GD.Print($"Sheep saved via 'S' key! ({SheepManager.Instance?.GetSheepCount() ?? 0} sheep)");
		}

		// Project mouse to ground plane at current height
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector3 from = _camera.ProjectRayOrigin(mousePos);
		Vector3 dir = _camera.ProjectRayNormal(mousePos);

		Plane ground = new Plane(Vector3.Up, -GlobalPosition.Y);
		if (!IntersectRayWithPlane(from, dir, ground, out var target)) return;

		// Move player so weapon tip touches the mouse location, only XZ
		Vector3 toTarget = (target - GlobalPosition).Normalized();
		var newPos = target - toTarget * WeaponLength;
		newPos.Y = GlobalPosition.Y;
		GlobalPosition = newPos;

		// Rotate player and weapon to face target, keep Y level
		LookAt(new Vector3(target.X, GlobalPosition.Y, target.Z), Vector3.Up);
		_weapon.GlobalPosition = GlobalPosition + toTarget * (WeaponLength * 0.5f);
		_weapon.LookAt(new Vector3(target.X, _weapon.GlobalPosition.Y, target.Z), Vector3.Up);

		// Change face sprite frame
		UpdateFace(toTarget);
	}

	private void UpdateFace(Vector3 dir)
	{
		if (_character == null) return;

		float angle = Mathf.RadToDeg(Mathf.Atan2(dir.X, dir.Z));
		int frame;

		if (Mathf.Abs(angle) < 45) frame = 2; // front
		else if (angle > 45 && angle < 135) frame = 3; // facing left
		else if (Mathf.Abs(angle) > 135) frame = 0; // back
		else frame = 1;  // facing right

		_character.Frame = frame;
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
