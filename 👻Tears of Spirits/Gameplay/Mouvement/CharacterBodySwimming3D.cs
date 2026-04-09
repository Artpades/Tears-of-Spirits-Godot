using Godot;
using System;
namespace GameplayToolkit.MovementAndPhysics
{
	// ── Implémentation CharacterBody3D ───────────────────────────────────────
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.CHARACTERMOVEMENT)]
#endif
	public partial class CharacterBodySwimming3D : SwimmingMovement
	{
		private CharacterBody3D _body;

		public override void _Ready()
		{
			base._Ready();
			_body = _parent as CharacterBody3D;
			if (_body is null)
			{
				GD.PrintErr($"{Name} : le parent doit être un CharacterBody3D.");
				SetPhysicsProcess(false);
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			if (_body is null) return;

			_body.Velocity = ComputeSwimVelocity3D(_body.Velocity, delta);
			_body.MoveAndSlide();  // pas de gravité car on ne l'ajoute jamais à Velocity
		}
	}
}
