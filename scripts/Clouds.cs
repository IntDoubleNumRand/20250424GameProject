using Godot;
using System;

public partial class Clouds : Node3D
{
	public override async void _Ready()
	{
		var shader = GD.Load<Shader>("res://shader/clouds_sky.gdshader");
		var mat = new ShaderMaterial { Shader = shader };
		var fnl = new FastNoiseLite();
		fnl.SetNoiseType(FastNoiseLite.NoiseType.TYPE_CELLULAR);
		fnl.SetCellularReturnType(FastNoiseLite.CellularReturnType.RETURN_DISTANCE);
		fnl.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.DISTANCE_EUCLIDEAN);
		fnl.SetCellularJitter(1.0f);
		var noiseTex = new NoiseTexture2D {
			Width = 512,
			Height = 512,
			Seamless = true,
			Normalize = true,
			Noise = fnl
		};
		await ToSignal(noiseTex, "changed");
		mat.SetShaderParameter("cloud_noise", noiseTex);
		GetNode<MeshInstance3D>("CloudPlane").MaterialOverride = mat;
	}
}
