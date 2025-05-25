using Godot;
using System.Collections.Generic;

public class WorldPathAdapter
{
	private readonly PathOnScreen _screenPath;
	private readonly ICoordinateMapper _mapper;

	public WorldPathAdapter(PathOnScreen screenPath, ICoordinateMapper mapper)
	{
		_screenPath = screenPath;
		_mapper = mapper;
	}

	public Vector3 GetStart() => _mapper.Map2Dto3D(_screenPath.Start);
	public Vector3 GetGoal() => _mapper.Map2Dto3D(_screenPath.Goal);

	public List<Vector3> GetMidPoints()
	{
		var result = new List<Vector3>();
		foreach (var pt in _screenPath.MidPoints)
			result.Add(_mapper.Map2Dto3D(pt));
		return result;
	}

	public List<Vector3> GetFullPath()
	{
		var path = new List<Vector3> { GetStart() };
		path.AddRange(GetMidPoints());
		path.Add(GetGoal());
		return path;
	}
}
