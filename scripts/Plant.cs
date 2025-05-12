using Godot;

public partial class Plant : Node3D
{
	public virtual void Reset() => Hide();
	public virtual void Activate(Vector3 pos)
	{
		GlobalPosition = pos;
		Show();
	}
}
