using Godot;
using System;

[Tool]
public partial class GroundSpawner : Node3D
{
	public override void _Ready()
	{
		if (Engine.IsEditorHint())
			SpawnOneGround();
	}

	private void SpawnOneGround()
	{
		GroundBuilder.Create(this)
			.SetWidth(15)
			.SetDepth(15)
			.SetCellSize(1.0f)
			.SetEnableHill(true)
			.SetHillHeight(3.5f)
			.SetPlateauT(0.7f)
			.SetFalloffExponent(0.6f)
			.SetColor(new Color(0.1f, 0.8f, 0.3f))
			.SetPosition(Vector3.Zero)
			.Build();
	}
}
