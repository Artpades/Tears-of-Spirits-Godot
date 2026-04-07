using GameplayToolkit;
using Godot;

namespace GameplayToolkit.MovementAndPhysics
{


	/// <summary>
	/// Gestion du recul pour un <see cref="RigidBody3D"/>.
	/// Supporte les modes Impulsion (instantané) et Continu.
	/// </summary>
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.RECOILRIGIDBODY3D)]
#endif
	public partial class KnockbackRigidBody3D : KnockbackMovement
	{
		private Vector3 _knockbackVelocity = Vector3.Zero;

		public override void _Ready()
		{
			if (Engine.IsEditorHint()) return;
			AnalyseParent();
		}

		public override void _Process(double delta)
		{
			if (Engine.IsEditorHint()) return;
			if (GetParent() is not RigidBody3D rigidBody3D) return;

			if (CanHaveKnockback) HandleKnockback(rigidBody3D);
		}

		// ─── API publique ───────────────────────────────────────────────
		public void ApplyKnockback(Vector3 direction)
			=> ApplyKnockback(direction, 1f);

		public void ApplyKnockback(Vector3 direction, float forceMultiplier)
		{
			if (!CanHaveKnockback) return;

			_knockbackVelocity = direction.Normalized() * BaseKnockbackForce * forceMultiplier;
			_isKnockbackActivated = true;
			_impulseApplied = false;
			_elapsedKnockbackTime = 0f;
		}

		// ─── Logique interne ────────────────────────────────────────────
		internal void HandleKnockback(RigidBody3D rigidBody3D)
		{
			if (!_isKnockbackActivated) return;

			if (Mode == RigidBodyMode.Impulse && !_impulseApplied)
			{
				Vector3 impulse = _knockbackVelocity * ForceMult;
				Vector3 torqueAxis = impulse.Cross(Vector3.Up).Normalized();
				rigidBody3D.ApplyImpulse(impulse);
				if (_addTorque) rigidBody3D.ApplyTorqueImpulse(torqueAxis * impulse.Length());
				_impulseApplied = true;
			}
			else if (Mode == RigidBodyMode.Continuous && !_impulseApplied)
			{
				rigidBody3D.ApplyCentralForce(_knockbackVelocity * ForceMult);
				if (_addTorque)
				{
					Vector3 torqueAxis = _knockbackVelocity.Cross(Vector3.Up);
					rigidBody3D.ApplyTorque(torqueAxis * _knockbackVelocity.Length() * ForceMult);
				}
				_impulseApplied = true;
			}
		}
	}
}