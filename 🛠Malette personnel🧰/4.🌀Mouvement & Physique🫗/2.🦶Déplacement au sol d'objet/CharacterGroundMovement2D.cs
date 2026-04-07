using GameplayToolkit;
using Godot;

namespace GameplayToolkit.MovementAndPhysics
{

	/// <summary>
	/// Mouvement au sol pour un <see cref="CharacterBody2D"/>.
	/// Gère la gravité, le saut et l'interpolation de vitesse horizontale en 2D.
	/// </summary>
#if TOOLS
	[GlobalClass, Tool]
#endif
	public partial class CharacterGroundMovement2D : GroundMovement
	{
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

			HandleGroundMovement(character2D, _inputDir, (float)delta);

			character2D.Velocity = _velocity;
			character2D.MoveAndSlide();

			if (DebugMode) character2D.QueueRedraw();
		}

		// ─── Logique ────────────────────────────────────────────────────
		internal void HandleGroundMovement(CharacterBody2D character2D, Vector2 wishedDirection, float delta)
		{
			_velocity = character2D.Velocity;

			// Gravité
			if (!character2D.IsOnFloor())
				_velocity += character2D.GetGravity() * delta * GravityRatio;

			// Saut
			if (_hasJumped && character2D.IsOnFloor())
			{
				_velocity.Y = -Mathf.Sqrt(2f * JumpVelocity * Pixel2DFactor * character2D.GetGravity().Y);
				_hasJumped = false;
			}

			// Interpolation horizontale
			_velocity.X = _velocity.Lerp(wishedDirection * maxSpeed * Pixel2DFactor, FactorSpeed * delta).X;
		}

		// ─── Debug ──────────────────────────────────────────────────────
		private void DrawDebugGizmos2D(Node2D node2D)
		{
			Vector2 start = Vector2.Zero;
			Vector2 end = start + _velocity;
			node2D.DrawLine(start, end, Colors.GreenYellow, 5f);
			node2D.DrawCircle(end, 7f, Colors.Aqua);
		}
	}
}
