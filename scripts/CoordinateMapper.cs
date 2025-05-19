using Godot;

public class CoordinateMapper : ICoordinateMapper
{
	private readonly Camera3D Camera;

	public CoordinateMapper(Camera3D camera)
	{
		Camera = camera;
	}

	public Vector3 Map2Dto3D(Vector2 point2D)
	{
		Vector3 origin = Camera.ProjectRayOrigin(point2D);
		Vector3 dirLocal = Camera.ProjectRayNormal(point2D).Normalized();
		Vector3 dirWorld = Camera.GlobalTransform.Basis * dirLocal;

		float t = -origin.Y / dirWorld.Y;
		return origin + dirWorld * t;
	}

	public Vector2 Map3Dto2D(Vector3 point3D)
	{
		return Camera.UnprojectPosition(point3D);
	}
}
