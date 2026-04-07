using GameplayToolkit;
using Godot;

namespace GameplayToolkit.MovementAndPhysics
{
	/// <summary>
	/// Classe intermédiaire partagée pour le mouvement au sol.
	/// Contient les paramètres communs (saut, gravité, rotation) et la logique de saut.
	/// </summary>
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.CHARACTERMOVEMENT)]
#endif
	public abstract partial class GroundMovement : PhysicsMovement
	{
		[ExportGroup("🌱Paramètre de Mouvement au sol")]
		[Export] public float JumpVelocity = 10f;
		[Export] public float GravityRatio = 5f;
		[Export] public float RotationSpeed = 20f;

		protected bool _hasJumped;

		/// <summary>
		/// Déclenche un saut si le personnage est au sol.
		/// </summary>
		public void Jump()
		{
			Node parent = GetParent();

			bool isOnFloor = parent switch
			{
				CharacterBody2D c2D => c2D.IsOnFloor(),
				CharacterBody3D c3D => c3D.IsOnFloor(),
				_ => false
			};

			if (isOnFloor) _hasJumped = true;
		}
	}
}

