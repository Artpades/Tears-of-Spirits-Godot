using GameplayToolkit;
using Godot;

namespace GameplayToolkit.MovementAndPhysics
{

	/// <summary>
	/// Gestion du recul pour un <see cref="RigidBody2D"/>.
	/// Supporte les modes Impulsion (instantané) et Continu.
	/// </summary>
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.RECOILRIGIDBODY3D)]
#endif
	public partial class KnockbackRigidBody2D : KnockbackMovement
	{
		private Vector2 _knockbackVelocity = Vector2.Zero;

		public override void _Ready()
		{
			if (Engine.IsEditorHint()) return;

			if (AnalyseParent() is Node2D node2D && DebugMode)
				node2D.Draw += () => DrawDebugGizmos2D(node2D);
		}

		public override void _Process(double delta)
		{
			if (Engine.IsEditorHint()) return;
			if (GetParent() is not RigidBody2D rigidBody2D) return;

			if (CanHaveKnockback) HandleKnockback(rigidBody2D);

			if (DebugMode) rigidBody2D.QueueRedraw();
		}

		// ─── API publique ───────────────────────────────────────────────
		public void ApplyKnockback(Vector2 direction)
			=> ApplyKnockback(direction, 1f);

		public void ApplyKnockback(Vector2 direction, float forceMultiplier)
		{
			if (!CanHaveKnockback) return;

			_knockbackVelocity = direction.Normalized() * BaseKnockbackForce * forceMultiplier * Pixel2DFactor;
			_isKnockbackActivated = true;
			_impulseApplied = false;
			_elapsedKnockbackTime = 0f;
		}

		// ─── Logique interne ────────────────────────────────────────────
		internal void HandleKnockback(RigidBody2D rigidBody2D)
		{
			if (!_isKnockbackActivated) return;

			if (Mode == RigidBodyMode.Impulse && !_impulseApplied)
			{
				float torqueAxis = _knockbackVelocity.Cross(Vector2.Up);
				rigidBody2D.ApplyImpulse(_knockbackVelocity);
				if (_addTorque) rigidBody2D.ApplyTorqueImpulse(torqueAxis * _knockbackVelocity.Length());
				_impulseApplied = true;
			}
			else if (Mode == RigidBodyMode.Continuous && !_impulseApplied)
			{
				rigidBody2D.ApplyCentralForce(_knockbackVelocity);
				if (_addTorque)
				{
					float torqueAxis = _knockbackVelocity.Cross(Vector2.Up);
					rigidBody2D.ApplyTorque(torqueAxis * _knockbackVelocity.Length());
				}
				_impulseApplied = true;
			}
		}

		// ─── Debug ──────────────────────────────────────────────────────
		private void DrawDebugGizmos2D(Node2D node2D)
		{
			if (!DebugMode) return;
			Vector2 start = Vector2.Zero;
			Vector2 end = start + _knockbackVelocity;
			node2D.DrawLine(start, end, Colors.GreenYellow, 5f);
			node2D.DrawCircle(end, 7f, Colors.Aqua);
		}
	}
}
