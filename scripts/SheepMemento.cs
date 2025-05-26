using System;
using System.Collections.Generic;

[Serializable]
public class SheepMemento
{
	public List<SheepRecord> SheepList { get; set; } = new List<SheepRecord>();
	public int SheepCount => SheepList?.Count ?? 0;
}

[Serializable]
public class SheepRecord
{
	public string Id { get; set; }
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }
	public DateTime? BirthTime { get; set; }
	public DateTime? DeathTime { get; set; }
}
