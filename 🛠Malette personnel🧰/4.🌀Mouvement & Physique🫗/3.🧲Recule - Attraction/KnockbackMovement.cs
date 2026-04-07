using GameplayToolkit;
using Godot;

namespace GameplayToolkit.MovementAndPhysics
{

	/// <summary>
	/// Classe intermédiaire partagée pour tous les systèmes de recul/attraction.
	/// Contient les paramètres exportés communs et les champs d'état internes.
	/// </summary>
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.RECOIL3D)]
#endif
	public abstract partial class KnockbackMovement : PhysicsMovement
	{
		[ExportCategory("🌬️Recoil et Attraction")]
		[Export] public bool CanHaveKnockback = true;
		[Export] public bool CanBeAttract;
		[Export] public bool GravityAffectKnockback = true;
		[Export] public float GravityRatio = 5f;

		[ExportSubgroup("⚙️Paramètre de Recule⚙️")]
		[Export] public float BaseKnockbackForce = 10f;
		[Export] public float KnockbackDuration = 0.5f;

		// ─── État interne partagé ───────────────────────────────────────
		protected bool _isKnockbackActivated;
		protected bool _impulseApplied;
		protected float _elapsedKnockbackTime;

		// ─── Mode RigidBody ─────────────────────────────────────────────
		public enum RigidBodyMode { Impulse, Continuous }

		[ExportSubgroup("⚙️Paramètre de Recule⚙️/🤾RigidBody")]
		[Export] public RigidBodyMode Mode = RigidBodyMode.Impulse;
		[Export] public bool _addTorque = true;
		[Export] public float ForceMult = 5f;
	}
}