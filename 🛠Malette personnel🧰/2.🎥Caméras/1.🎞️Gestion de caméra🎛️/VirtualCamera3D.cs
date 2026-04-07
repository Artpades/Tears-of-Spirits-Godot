using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
namespace GameplayToolkit.Cameras;

[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.VIRTUALCAM)
]
public partial class VirtualCamera3D : Camera3D
{
	// TODO : Faire en sorte de pouvoir zoom et d'analyser la distance entre la caméra et sa cible et faire des limite de distance min et max
	 

	#region 🎯 Follow & Look At System

	[Export] public Node3D FollowTarget;

	[Export] public Vector3 FollowOffset = Vector3.Zero;

	[Export] public float FollowSmoothness = 5.0f;


	#endregion


	#region ⚙️ Lifecycle

	public override void _Process(double delta)
	{
		ProcessShakeEffect((float)delta);
		ProcessLookAtEffect3D((float)delta);
		// Système Follow Target
		if (FollowTarget != null && IsInstanceValid(FollowTarget))
		{
			ProcessFollowTarget((float)delta);
		}
		
	}
	public void RotateCam(Vector2 newPos)
	{
		Rotation = new(Rotation.X + newPos.Y, Rotation.Y + newPos.X, 0);
	}
	#endregion

	#region 🎯 Follow & Look At Logic

	private void ProcessFollowTarget(float delta)
	{
		// Calculer la position cible avec l'offset
		Vector3 targetPosition = FollowTarget.GlobalPosition + FollowOffset;

		// Interpoler vers la position cible
		GlobalPosition = GlobalPosition.Lerp(targetPosition, FollowSmoothness * delta);
	}

	#endregion

}
