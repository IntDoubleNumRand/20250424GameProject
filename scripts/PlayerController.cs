using Godot;
using System;

public partial class PlayerController : Node3D
{
	[Export] public NodePath CharacterPath = "Character";
	[Export] public NodePath WeaponMeshPath = "WeaponMesh";
	[Export] public float WeaponLength = 2f;

	private Sprite3D _character;
	private MeshInstance3D _weapon;
	private Camera3D _camera;

	public override void _Ready()
	{
		_character = GetNode<Sprite3D>(CharacterPath);
		_weapon = GetNode<MeshInstance3D>(WeaponMeshPath);

		// Automatically get the active Camera3D from the viewport
		_camera = GetViewport().GetCamera3D();
		if (_camera == null)
			GD.PrintErr("No Camera3D found in the viewport.");
	}

	public override void _Process(double delta)
	{
		if (_camera == null) return;

		// Project mouse position to world space
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector3 from = _camera.ProjectRayOrigin(mousePos);
		Vector3 dir = _camera.ProjectRayNormal(mousePos);
		Plane groundPlane = new Plane(Vector3.Up, 0f);

		if (!IntersectRayWithPlane(from, dir, groundPlane, out Vector3 target))
			return;

		// Move the PlayerController node so the weapon's tip is at the target
		Vector3 toTarget = (target - GlobalPosition).Normalized();
		GlobalPosition = target - toTarget * WeaponLength;

		// Rotate to face the mouse point
		LookAt(target, Vector3.Up);

		// Position the weapon between character and cursor
		_weapon.GlobalPosition = GlobalPosition + toTarget * (WeaponLength * 0.5f);
		_weapon.LookAt(target, Vector3.Up);

		// Update the character's facing sprite
		UpdateFace(toTarget);
	}

	private void UpdateFace(Vector3 dir)
	{
		float angle = Mathf.RadToDeg(Mathf.Atan2(dir.X, dir.Z));
		int frame;

		if (Mathf.Abs(angle) < 45)           frame = 0; // Front
		else if (angle > 45 && angle < 135)  frame = 1; // Left
		else if (Mathf.Abs(angle) > 135)     frame = 2; // Back
		else                                 frame = 3; // Right

		_character.Frame = frame;
	}

	private bool IntersectRayWithPlane(Vector3 from, Vector3 dir, Plane plane, out Vector3 intersection)
	{
		float denom = plane.Normal.Dot(dir);
		if (Mathf.Abs(denom) < 0.0001f)
		{
			intersection = Vector3.Zero;
			return false;
		}

		float t = -(plane.Normal.Dot(from) + plane.D) / denom;
		if (t < 0)
		{
			intersection = Vector3.Zero;
			return false;
		}

		intersection = from + dir * t;
		return true;
	}
}
