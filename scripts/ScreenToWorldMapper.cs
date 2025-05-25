using Godot;

public class ScreenToWorldMapper : ICoordinateMapper
{
	private readonly Camera3D _camera;

	public ScreenToWorldMapper(Camera3D camera)
	{
		_camera = camera;
	}

	public Vector3 Map2Dto3D(Vector2 screenPoint)
	{
		Vector3 rayOrigin = _camera.ProjectRayOrigin(screenPoint);
		Vector3 rayDir = _camera.ProjectRayNormal(screenPoint);

		if (Mathf.Abs(rayDir.Y) < 1e-6 )
			throw new System.Exception("Ray is parallel to ground plane!");

		float t = -rayOrigin.Y / rayDir.Y;

		if (t == 0)
			throw new System.Exception("Intersection is behind the camera. Fix camera rotation or screen mapping!");

		Vector3 hit = rayOrigin + rayDir * t;
		Vector3 ground_hit = new Vector3(hit.X, 0, hit.Z);

		return ground_hit;
	}
}
