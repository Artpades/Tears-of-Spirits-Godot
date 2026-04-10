using GameplayToolkit;
using Godot;

namespace GameplayToolkit.MovementAndPhysics
{

	/// <summary>
	/// Classe de base commune pour tous les systèmes de déplacement physique.
	/// Contient les propriétés partagées et l'analyse du nœud parent.
	/// </summary>
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.RIGIDBODYMOVEMENT)]
#endif
	public abstract partial class PhysicsMovement : Node
	{
		[ExportCategory("Debug"), Export] internal bool DebugMode;

		[ExportCategory("📦Propriétés de déplacement")]
		[Export] public float maxSpeed = 5.0f;
		[Export] public float FactorSpeed = 5f;

		protected float Pixel2DFactor = 64f;

		private float _currentSpeed;
		protected float CurrentSpeed
		{
			get => _currentSpeed;
			set => _currentSpeed = Mathf.Clamp(value, 0, maxSpeed);
		}

		public Vector2 _inputDir;

		// ─── Entrées ────────────────────────────────────────────────────
		public void MoveCharacter(Vector2 direction) => _inputDir = direction;
		public void MoveOnAxeX(float horizontalInput) => _inputDir.X = horizontalInput;
		public void MoveOnAxeY(float verticalInput) => _inputDir.Y = verticalInput;

		// ─── Utilitaire parent ──────────────────────────────────────────
		protected Node AnalyseParent()
		{
			Node parent = GetParent();

			if (parent is CharacterBody2D or CharacterBody3D or RigidBody2D or RigidBody3D)
				return parent;

			GD.PrintErr($"Le parent de {Name} doit être un CharacterBody2D, CharacterBody3D, RigidBody2D ou RigidBody3D.");
			CustomLogger.Log(Name, IconPaths.RIGIDBODYMOVEMENT,
				$"Le parent de {Name} doit être un CharacterBody2D, CharacterBody3D, RigidBody2D ou RigidBody3D.");
			SetProcess(false);
			return null;
		}
	}
}
