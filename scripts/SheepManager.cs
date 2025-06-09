using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class SheepManager : Node
{
	public static SheepManager Instance { get; private set; }
	private string SaveFilePath => "res://saves/sheep_save.json";

	private Dictionary<string, SheepRecord> _sheepRecords = new();
	
	public int TotalSheep { get; private set; }
	public int SheepReachedGoal { get; private set; }
	public bool GameEnded { get; private set; } = false;

	public void InitializeSheepCount(int total)
	{
		TotalSheep = total;
		SheepReachedGoal = 0;
		GameEnded = false;
	}

	public void RecordSheepReached(string id)
	{
		if (GameEnded) return;
		SheepReachedGoal++;
		GD.Print($"{id} reached goal. Total: {SheepReachedGoal}");
		if (SheepReachedGoal >= TotalSheep)
		{
			GameEnded = true;
			ShowVictoryScreen();
		}
	}

	public new void RecordSheepDeath(string id)
	{
		if (GameEnded) return;
		
		if (_sheepRecords.TryGetValue(id, out var record))
		{
			record.DeathTime = DateTime.UtcNow;
			SaveSheep();
		}
		
		GameEnded = true;
		ShowGameOverScreen();
	}
	
	public void ResetGameState()
	{
		TotalSheep = 0;
		SheepReachedGoal = 0;
		GameEnded = false;
		_sheepRecords.Clear();
		SaveSheep();
	}

	private void ShowVictoryScreen()
	{
		GetTree().ChangeSceneToFile("res://scenes/VictoryScreen.tscn");
	}

	private void ShowGameOverScreen()
	{
		GetTree().ChangeSceneToFile("res://scenes/GameOverScreen.tscn");
	}

	public override void _Ready()
	{
		Instance = this;
		if (Godot.FileAccess.FileExists(SaveFilePath))
			LoadSheep();
	}

	public void RegisterSheep(string id, Vector3 pos)
	{
		_sheepRecords[id] = new SheepRecord {
			Id = id,
			X = pos.X,
			Y = pos.Y,
			Z = pos.Z,
			BirthTime = DateTime.UtcNow,
			DeathTime = null
		};
		SaveSheep();
	}

	public void SaveSheep()
	{
		var memento = new SheepMemento();
		memento.SheepList.AddRange(_sheepRecords.Values);
		var json = JsonSerializer.Serialize(memento, new JsonSerializerOptions { WriteIndented = true });
		using var file = Godot.FileAccess.Open(SaveFilePath, Godot.FileAccess.ModeFlags.Write);
		file.StoreString(json);
		GD.Print("Sheep saved!");
	}

	public void LoadSheep()
	{
		if (!Godot.FileAccess.FileExists(SaveFilePath)) return;
		using var file = Godot.FileAccess.Open(SaveFilePath, Godot.FileAccess.ModeFlags.Read);
		var json = file.GetAsText();
		try
		{
			var memento = JsonSerializer.Deserialize<SheepMemento>(json);
			_sheepRecords.Clear();
			foreach (var r in memento.SheepList)
				_sheepRecords[r.Id] = r;
			GD.Print($"Loaded {memento.SheepCount} sheep from save.");
		}
		catch (Exception e)
		{
			GD.PrintErr("Failed to load sheep save: " + e);
		}
	}

	public int GetSheepCount() => _sheepRecords.Count;
}
