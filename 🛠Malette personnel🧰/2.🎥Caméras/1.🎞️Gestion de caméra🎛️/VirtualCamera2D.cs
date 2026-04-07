using Godot;
using System;
namespace GameplayToolkit.Cameras;

[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.VIRTUALCAM)
]
public partial class VirtualCamera2D : Camera2D
{

	#region 🎯 Follow & Look At System

	[Export] public Node2D FollowTarget;

	[Export] public Vector2 FollowOffset = Vector2.Zero;

	[Export] public float FollowSmoothness = 5.0f;

	#endregion


	#region ⚙️ Lifecycle

	public override void _Process(double delta)
	{
		ProcessShakeEffect((float)delta);
		ProcessEffect2D((float)delta);

	}

	#endregion

	
}
