using Godot;
using System;

public interface ICoordinateMapper
{
	Vector3 Map2Dto3D(Vector2 point2D);
}
