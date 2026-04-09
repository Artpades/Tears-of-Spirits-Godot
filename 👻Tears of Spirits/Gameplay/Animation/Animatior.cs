using Godot;
using System;


#if TOOLS
[GlobalClass, Tool]
#endif
public partial class Animatior : Node3D
{
	[Export] public AnimationPlayer playerAnimation;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerAnimation.Play("Armature|ArmatureAction_001");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
