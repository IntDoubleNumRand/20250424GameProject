using Godot;

public class ScreenToWorldMapper : ICoordinateMapper
{
	private readonly Camera3D _camera;

	public ScreenToWorldMapper(Camera3D camera)
	{
		_camera = camera;
	}

	// Maps a 2D screen pixel to a 3D point on the ground plane (Y=0), in XZ coordinates
	public Vector3 Map2Dto3D(Vector2 screenPoint)
	{
		// The screenPoint should be in viewport coordinates (pixels)
		// If your UI is at a different resolution, rescale accordingly

		// Get ray from camera through this pixel
		Vector3 rayOrigin = _camera.ProjectRayOrigin(screenPoint);
		Vector3 rayDir = _camera.ProjectRayNormal(screenPoint);

		// Check if rayDir is not pointing downward
		if (Mathf.Abs(rayDir.Y) < 1e-6)
			throw new System.Exception("Ray is parallel to ground plane!");

		// Find intersection with Y=0 (ground plane)
		float t = -rayOrigin.Y / rayDir.Y;

		if (t < 0)
			throw new System.Exception("Intersection is behind the camera.");

		Vector3 intersection = rayOrigin + rayDir * t;
		// Return in XZ, with Y always 0
		return new Vector3(intersection.X, 0, intersection.Z);
	}
}
