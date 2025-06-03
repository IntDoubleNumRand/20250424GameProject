using Godot;
using System;

public class PlayerFacade
{
	private Camera3D _camera;
	private Viewport _viewport;
	private World3D _world;
	
	public PlayerFacade(Camera3D camera, Viewport viewport, World3D world){
		_camera = camera;
		_viewport = viewport;
		_world = world;
	}
	
	public void TryPickUpPlant(Vector2 mousePos)
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		//GD.Print("Input action 3: " + Input.MouseMode);
		var from = _camera.ProjectRayOrigin(mousePos);
		var to   = from + _camera.ProjectRayNormal(mousePos) * 1000f;

		var space = _world.DirectSpaceState;
		var query = new PhysicsRayQueryParameters3D
		{
			From = from,
			To = to,
			CollisionMask = 1 << 1
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
		Input.MouseMode = Input.MouseModeEnum.Visible;
		//GD.Print("Input action 4: " + Input.MouseMode);
	}
	
	public Wolf TryHitWolf(Vector2 mousePos)
	{
		var from = _camera.ProjectRayOrigin(mousePos);
		var to   = from + _camera.ProjectRayNormal(mousePos) * 1000f;

		var space = _world.DirectSpaceState;
		var query = new PhysicsRayQueryParameters3D
		{
			From = from,
			To = to,
			CollisionMask = 1 << 1
		};

		var result = space.IntersectRay(query);

		if (result.Count > 0)
		{
			var collider = result["collider"].As<Node>();
			if (collider is Wolf wolf)
			{
				GD.Print($"Wolf {wolf.Name} hit at {mousePos}");
				return wolf;
			}
		}
		return null;
	}
}
