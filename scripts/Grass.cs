using Godot;
using System;

public partial class Grass : Plant
{
	public override void _Ready()
	{
		AddToGroup("Plant");
	}
}
