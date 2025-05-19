using Godot;
using System;

public interface ICoordinateMapper
{
	Vector3 Map2Dto3D(Vector2 point2D);
	Vector2 Map3Dto2D(Vector3 point3D);
}
