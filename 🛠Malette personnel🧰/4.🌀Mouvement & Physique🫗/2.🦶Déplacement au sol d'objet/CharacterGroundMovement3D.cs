using GameplayToolkit;
using Godot;

namespace GameplayToolkit.MovementAndPhysics
{

	/// <summary>
	/// Mouvement au sol pour un <see cref="CharacterBody3D"/>.
	/// Gère la gravité, le saut, la rotation vers la direction de déplacement et
	/// l'interpolation de vitesse horizontale relative à la caméra active.
	/// </summary>
#if TOOLS
	[GlobalClass, Tool]
#endif
	public partial class CharacterGroundMovement3D : GroundMovement
	{
		private Vector3 _velocity;
		private Vector3 _currentVelocityHorizontal = Vector3.Zero;

		public override void _Ready()
		{
			if (Engine.IsEditorHint()) return;
			AnalyseParent(); // Valide le parent au démarrage
		}

		public override void _Process(double delta)
		{
			if (Engine.IsEditorHint()) return;
			if (GetParent() is not CharacterBody3D character3D) return;

			HandleGroundMovement(character3D, _inputDir, (float)delta);

			character3D.Velocity = _velocity;
			character3D.MoveAndSlide();
		}

		// ─── Logique ────────────────────────────────────────────────────
		internal void HandleGroundMovement(CharacterBody3D character3D, Vector2 wishedDirection, float delta)
		{
			_velocity = character3D.Velocity;

			// Gravité
			if (!character3D.IsOnFloor())
				_velocity += character3D.GetGravity() * delta * GravityRatio;

			// Saut
			if (_hasJumped && character3D.IsOnFloor())
			{
				_velocity.Y = JumpVelocity;
				_hasJumped = false;
			}

			// Direction relative à la caméra
			Vector3 direction = Vector3.Zero;

			if (_inputDir.LengthSquared() > 0.01f)
			{
				Camera3D cam = GetViewport().GetCamera3D();

				Vector3 camForward = -cam.GlobalTransform.Basis.Z;
				Vector3 camRight = cam.GlobalTransform.Basis.X;
				camForward.Y = 0f;
				camRight.Y = 0f;
				camForward = camForward.Normalized();
				camRight = camRight.Normalized();

				direction = (camForward * -_inputDir.Y + camRight * _inputDir.X).Normalized();

				// Rotation du personnage vers sa direction de déplacement
				if (direction.LengthSquared() > 0.01f)
				{
					float targetYaw = Mathf.Atan2(direction.X, direction.Z);
					Vector3 rot = character3D.Rotation;
					rot.Y = Mathf.LerpAngle(rot.Y, targetYaw, RotationSpeed * delta);
					character3D.Rotation = rot;
				}
			}

			// Interpolation horizontale
			_currentVelocityHorizontal = _currentVelocityHorizontal.Lerp(
				direction * maxSpeed, FactorSpeed * delta);

			_velocity.X = _currentVelocityHorizontal.X;
			_velocity.Z = _currentVelocityHorizontal.Z;
		}
	}
}
