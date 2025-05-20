using Godot;

public class ScreenToWorldMapper : ICoordinateMapper
{
	private readonly Camera3D _camera;

	public ScreenToWorldMapper(Camera3D camera)
	{
		_camera = camera;
	}

	public Vector3 Map2Dto3D(Vector2 point2D)
	{
		Vector2 originalScreenSize = new Vector2(1299f, 648f);
		// Convert point from original screen space to current viewport
		Vector2 currentScreenSize = _camera.GetViewport().GetVisibleRect().Size;
		float scaleX = currentScreenSize.X / originalScreenSize.X;
		float scaleY = currentScreenSize.Y / originalScreenSize.Y;
		Vector2 mappedPoint = new Vector2(point2D.X * scaleX, point2D.Y * scaleY);

		// Project ray from scaled screen point
		Vector3 origin = _camera.ProjectRayOrigin(mappedPoint);
		Vector3 direction = _camera.ProjectRayNormal(mappedPoint);

		// Intersect with ground plane at Y = 0
		Plane groundPlane = new Plane(Vector3.Up, -0f);
		float denom = groundPlane.Normal.Dot(direction);
		if (Mathf.Abs(denom) < 0.0001f)
		{
			GD.PrintErr("Ray is nearly parallel to ground plane.");
			return Vector3.Zero;
		}

		float t = -(groundPlane.Normal.Dot(origin) + groundPlane.D) / denom;
		return origin + direction * t;
	}

}
