using GameplayToolkit;
using GameplayToolkit.UIUX;
using Godot;
using System;
#if TOOLS
[GlobalClass, Tool]
#endif
public partial class FeuFollet : Node3D
{
	[Export] private Label3D label3D;
	[Export] public Dialogue Dialogue;
	[Export] private Color outlineColor1 = new(1, 1, 0); // Jaune par défaut
	[Export] private Color outlineColor2 = new(1, 0.5f, 0); // Orange par défaut
	[Export] private float tweenDuration = 1.0f; // Durée en secondes

	private Tween outlineTween;
	private void OnEnteredArea(Node3D body)
	{
		if (body is CharacterBody3D)
		{
			label3D.Show();
		}
	}

	private void OnExitedArea(Node3D body)
	{
		if (body is CharacterBody3D)
		{
			label3D.Hide();

		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (label3D != null)
		{
			StartOutlineTween();
		}
	}

	private void StartOutlineTween()
	{
		// Arrêter le tween précédent s'il existe
		if (outlineTween != null && outlineTween.IsValid())
		{
			outlineTween.Kill();
		}

		// Créer un nouveau tween
		outlineTween = CreateTween();

		// Configurer le tween en boucle ping-pong infinie
		outlineTween.SetLoops(); // Boucle infinie
		outlineTween.SetTrans(Tween.TransitionType.Sine); // Transition douce
		outlineTween.SetEase(Tween.EaseType.InOut); // Ease in-out pour un effet fluide

		// Animer la propriété OutlineModulate
		outlineTween.TweenProperty(label3D, "outline_modulate", outlineColor2, tweenDuration);
		outlineTween.TweenProperty(label3D, "outline_modulate", outlineColor1, tweenDuration);
	}

	// Appelé quand le node quitte l'arbre de scène
	public override void _ExitTree()
	{
		// Nettoyer le tween
		if (outlineTween != null && outlineTween.IsValid())
		{
			outlineTween.Kill();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}