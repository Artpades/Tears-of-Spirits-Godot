using GameplayToolkit;
using Godot;

namespace GameplayToolkit.MovementAndPhysics
{
	/// <summary>
	/// Système de déplacement de nage : 3 axes (X, Y, Z ou X/Y en 2D),
	/// sans gravité, avec montée/descente. Compatible CharacterBody et RigidBody.
	/// </summary>
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.CHARACTERMOVEMENT)]
#endif
	public abstract partial class SwimmingMovement : PhysicsMovement
	{
		[ExportGroup("🌊 Paramètres de nage")]
		[Export] public float BuoyancyDamping = 3f;
		[Export] public float VerticalSpeed = 4f;
		[Export] public Camera3D Camera;             // optionnelle, fallback sur la caméra active

		protected float _verticalInput;

		// ─── Entrées verticales ─────────────────────────────────────────
		public void Dive(float verticalInput) => _verticalInput = verticalInput;
		public void DiveUp(float upInput) => _verticalInput = upInput;
		public void DiveDown(float downInput) => _verticalInput = -downInput;
		public void SwimUp() => _verticalInput = 1f;
		public void SwimDown() => _verticalInput = -1f;
		public void StopVertical() => _verticalInput = 0f;

		protected Node _parent;

		public override void _Ready()
		{
			_parent = AnalyseParent();
		}

		// ─── Résolution de la caméra ────────────────────────────────────
		protected Camera3D ResolveCamera()
		{
			if (Camera is not null) return Camera;

			Camera3D active = GetViewport().GetCamera3D();
			if (active is not null) return active;

			GD.PrintErr($"{Name} : aucune caméra trouvée. Assignez-en une via l'export ou assurez-vous qu'une Camera3D est active.");
			return null;
		}

		// ─── Physique partagée ──────────────────────────────────────────
		protected Vector3 ComputeSwimVelocity3D(Vector3 currentVelocity, double delta)
		{
			float dt = (float)delta;

			Vector3 horizontalDirection = Vector3.Zero;

			if (_inputDir.LengthSquared() > 0.01f)
			{
				Camera3D cam = ResolveCamera();
				if (cam is null) return currentVelocity;

				Vector3 camForward = -cam.GlobalTransform.Basis.Z;
				Vector3 camRight = cam.GlobalTransform.Basis.X;
				camForward.Y = 0f;
				camRight.Y = 0f;
				camForward = camForward.Normalized();
				camRight = camRight.Normalized();

				horizontalDirection = (camForward * -_inputDir.Y + camRight * _inputDir.X).Normalized();
			}

			// Axe vertical indépendant de la caméra (montée/descente monde)
			Vector3 target = horizontalDirection * maxSpeed + Vector3.Up * (_verticalInput * VerticalSpeed);

			return currentVelocity.Lerp(target, BuoyancyDamping * dt);
		}

		protected Vector2 ComputeSwimVelocity2D(Vector2 currentVelocity, double delta)
		{
			float dt = (float)delta;

			Vector2 direction = new Vector2(_inputDir.X, -_verticalInput).Normalized();
			Vector2 target = direction * maxSpeed;

			return currentVelocity.Lerp(target, BuoyancyDamping * dt);
		}
	}
}