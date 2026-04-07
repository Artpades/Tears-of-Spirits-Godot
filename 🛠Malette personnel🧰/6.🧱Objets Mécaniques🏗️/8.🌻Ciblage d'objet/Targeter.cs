using GameplayToolkit.EnvironmentAndNetwork;
using Godot;
using System;
namespace GameplayToolkit.ObjectInteractionAndInfluencer
{

#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.TARGETER)]
#endif
	public partial class Targeter : Node
	{
		public Node NodeToApplyEffect => GetParent();

		[Export] public NodePath NodeToTarget;
		[Export] public bool Activated;
		[ExportCategory("Interpolation")]
		[Export] public bool AddLerp;
		[Export] public float LerpSpeed = 5f;

		public override void _Ready() { }

		public override void _Process(double delta)
		{
			if (Activated)
			{
				if (!AddLerp)
				{
					if (NodeToApplyEffect is Node2D node2D && GetNode<Node2D>(NodeToTarget) is not null)
					{
						node2D.LookAt(GetNode<Node2D>(NodeToTarget).GlobalPosition);
					}
					else if (NodeToApplyEffect is Node3D node3D && GetNode<Node3D>(NodeToTarget) is not null)
					{
						node3D.LookAt(GetNode<Node3D>(NodeToTarget).GlobalPosition);
					}
				}
				else
				{
					if (NodeToApplyEffect is Node2D node2D && GetNode<Node2D>(NodeToTarget) is Node2D target2D)
					{
						// Calcule l'angle vers la cible
						Vector2 direction = target2D.GlobalPosition - node2D.GlobalPosition;
						float targetAngle = direction.Angle() + Mathf.Pi / 2f; // +90° car LookAt pointe l'axe Y

						// Interpolation de l'angle actuel vers l'angle cible
						node2D.Rotation = Mathf.LerpAngle(node2D.Rotation, targetAngle, LerpSpeed * (float)delta);
					}
					else if (NodeToApplyEffect is Node3D node3D && GetNode<Node3D>(NodeToTarget) is Node3D target3D)
					{
						// Sauvegarde la rotation actuelle
						Basis currentBasis = node3D.GlobalBasis;

						// Applique LookAt pour obtenir la rotation cible
						node3D.LookAt(target3D.GlobalPosition);
						Basis targetBasis = node3D.GlobalBasis;

						// Restaure la rotation actuelle, puis interpole vers la cible
						node3D.GlobalBasis = currentBasis.Slerp(targetBasis, LerpSpeed * (float)delta);
					}
				}
			}
		}
		public void SetActiveTargeting(bool isActivated) => Activated = isActivated;

	}
}