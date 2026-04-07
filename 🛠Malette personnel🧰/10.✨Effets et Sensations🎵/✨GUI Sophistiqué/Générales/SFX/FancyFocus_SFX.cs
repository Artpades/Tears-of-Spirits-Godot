using Godot;
using System;
namespace GameplayToolkit.VisualAndSoundEffects.GUI;

public partial class FancyGUI
{
	[ExportGroup("SFX")]

	[Export] public AudioStreamMP3 SfxOnHover;
	[Export] public float PitchVariation = 0.2f;
	private AudioStreamPlayer _audioStream;
	private readonly Random _random = new();

	private void HandleSound()
	{
		// Crée un nouveau lecteur audio et configure son stream.
		_audioStream = new AudioStreamPlayer
		{
			Stream = SfxOnHover
		};
		// Ajoute le lecteur comme enfant du GUI pour qu'il puisse jouer le son.
		CallDeferred(MethodName.AddAudioChild);
		// Connecte les événements de focus pour déclencher le son.
		FocusParent.FocusEntered += PlayClickSound;

		// Connecte les événements de souris pour déclencher le son.
		FocusParent.MouseEntered += PlayClickSound;
	}
	internal void AddAudioChild() => FocusParent.AddChild(_audioStream);
	private void PlayClickSound()
	{
		// Vérifie que le lecteur audio et le son sont bien configurés avant de jouer.
		if (_audioStream != null && SfxOnHover != null && _audioStream.IsInsideTree())
		{
			// Génère un pitch aléatoire : 1.0 ± (valeur entre -PitchVariation et +PitchVariation).
			float randomPitch = 1.0f + ((float)_random.NextDouble() * 2 - 1) * PitchVariation;
			_audioStream.PitchScale = randomPitch;

			// Lance la lecture du son avec le pitch aléatoire appliqué.
			_audioStream.Play();
		}
	}
}
