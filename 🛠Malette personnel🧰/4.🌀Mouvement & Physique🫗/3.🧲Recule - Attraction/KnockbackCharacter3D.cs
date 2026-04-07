using GameplayToolkit;
using Godot;

namespace GameplayToolkit.MovementAndPhysics
{

	/// <summary>
	/// Gestion du recul pour un <see cref="CharacterBody3D"/>.
	/// </summary>
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.RECOILCHARACTER3D)]
#endif
	public partial class KnockbackCharacter3D : KnockbackMovement
	{
		private Vector3 _knockbackVelocity = Vector3.Zero;
		private Vector3 _velocity;

		public override void _Ready()
		{
			if (Engine.IsEditorHint()) return;
			AnalyseParent();
		}

		public override void _Process(double delta)
		{
			if (Engine.IsEditorHint()) return;
			if (GetParent() is not CharacterBody3D character3D) return;

			if (CanHaveKnockback) HandleKnockback(character3D, (float)delta);

			character3D.Velocity = _velocity;
			character3D.MoveAndSlide();
		}

		// ─── API publique ───────────────────────────────────────────────
		public void ApplyKnockback(Vector3 direction)
			=> ApplyKnockback(direction, 1f);

		public void ApplyKnockback(Vector3 direction, float forceMultiplier)
		{
			if (!CanHaveKnockback) return;

			if (AnalyseParent() is CharacterBody3D character3D)
				_velocity = character3D.Velocity;

			_knockbackVelocity = direction.Normalized() * BaseKnockbackForce * forceMultiplier;
			_isKnockbackActivated = true;
			_impulseApplied = false;
			_elapsedKnockbackTime = 0f;
		}

		// ─── Logique interne ────────────────────────────────────────────
		internal void HandleKnockback(CharacterBody3D character3D, float delta)
		{
			if (!_isKnockbackActivated) return;

			_velocity = character3D.Velocity;

			if (GravityAffectKnockback && !character3D.IsOnFloor())
				_knockbackVelocity += character3D.GetGravity() * delta * GravityRatio;

			_elapsedKnockbackTime += delta;

			if (_elapsedKnockbackTime >= KnockbackDuration)
			{
				bool canStop = !GravityAffectKnockback || character3D.IsOnFloor();
				if (canStop)
				{
					_isKnockbackActivated = false;
					_knockbackVelocity = _knockbackVelocity.Slerp(Vector3.Zero, 0.1f);
					if (_knockbackVelocity == Vector3.Zero)
						_elapsedKnockbackTime = 0f;
				}
			}

			_velocity = _knockbackVelocity;
		}
	}
}
