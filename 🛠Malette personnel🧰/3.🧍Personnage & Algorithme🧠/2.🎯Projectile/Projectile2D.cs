using Godot;
using System;
namespace GameplayToolkit.CharacterAndAlgorithm
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.PROJECTILE)]
#endif
	public partial class Projectile2D : Node2D
	{
		[Export] bool MoveInLine = false;
		[Export] public float Speed = 100f;
		[Export] public float LifeTime = 5f; // Durée de vie du projectile en secondes


		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			// Progress Ratio ++
			if (GetParent() is PathFollow2D pathFollow)
			{
				// Avance le projectile le long du chemin
				pathFollow.ProgressRatio += (float)delta / LifeTime;

				// Libère le projectile une fois qu'il a atteint la fin du chemin
				if (!pathFollow.Loop && pathFollow.ProgressRatio >= 1f) if (!Engine.IsEditorHint()) QueueFree();
			}

			if (Engine.IsEditorHint() || GetParent() is PathFollow2D) return;
			if (MoveInLine) GlobalPosition += new Vector2((float)delta * Speed, 0); // Déplace le projectile vers la droite à une vitesse de 100 pixels par seconde
		}
	}

}

