using Godot;
using System;

#if TOOLS
[GlobalClass, Tool]
#endif
public partial class PathFollower : PathFollow3D
{
	[Export] public float MoveSpeed = 0.05f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ProgressRatio += (float)delta * MoveSpeed;
	}
}
