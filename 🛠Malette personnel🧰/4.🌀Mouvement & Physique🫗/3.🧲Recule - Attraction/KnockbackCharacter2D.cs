using GameplayToolkit;
using Godot;

namespace GameplayToolkit.MovementAndPhysics
{

	/// <summary>
	/// Gestion du recul pour un <see cref="CharacterBody2D"/>.
	/// </summary>
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.RECOILCHARACTER3D)]
#endif
	public partial class KnockbackCharacter2D : KnockbackMovement
	{
		private Vector2 _knockbackVelocity = Vector2.Zero;
		private Vector2 _velocity;

		public override void _Ready()
		{
			if (Engine.IsEditorHint()) return;

			if (AnalyseParent() is Node2D node2D && DebugMode)
				node2D.Draw += () => DrawDebugGizmos2D(node2D);
		}

		public override void _Process(double delta)
		{
			if (Engine.IsEditorHint()) return;
			if (GetParent() is not CharacterBody2D character2D) return;

			if (CanHaveKnockback) HandleKnockback(character2D, (float)delta);

			character2D.Velocity = _velocity;
			character2D.MoveAndSlide();

			if (DebugMode) character2D.QueueRedraw();
		}

		// ─── API publique ───────────────────────────────────────────────
		public void ApplyKnockback(Vector2 direction)
			=> ApplyKnockback(direction, 1f);

		public void ApplyKnockback(Vector2 direction, float forceMultiplier)
		{
			if (!CanHaveKnockback) return;

			if (AnalyseParent() is CharacterBody2D character2D)
				_velocity = character2D.Velocity;

			_knockbackVelocity = direction.Normalized() * BaseKnockbackForce * forceMultiplier * Pixel2DFactor;
			_isKnockbackActivated = true;
			_impulseApplied = false;
			_elapsedKnockbackTime = 0f;
		}

		// ─── Logique interne ────────────────────────────────────────────
		internal void HandleKnockback(CharacterBody2D character2D, float delta)
		{
			if (!_isKnockbackActivated) return;

			_velocity = character2D.Velocity;

			if (GravityAffectKnockback && !character2D.IsOnFloor())
				_knockbackVelocity += character2D.GetGravity() * delta * GravityRatio;

			_elapsedKnockbackTime += delta;

			if (_elapsedKnockbackTime >= KnockbackDuration)
			{
				bool canStop = !GravityAffectKnockback || character2D.IsOnFloor();
				if (canStop)
				{
					_isKnockbackActivated = false;
					_knockbackVelocity = _knockbackVelocity.Slerp(Vector2.Zero, 0.1f);
					if (_knockbackVelocity == Vector2.Zero)
						_elapsedKnockbackTime = 0f;
				}
			}

			_velocity = _knockbackVelocity;
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
