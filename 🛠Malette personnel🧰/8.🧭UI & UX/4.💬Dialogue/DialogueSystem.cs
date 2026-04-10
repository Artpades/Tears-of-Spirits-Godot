using Godot;
using Godot.Collections;
using System;

namespace GameplayToolkit.UIUX;

[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.DIALOGUESYSTEM)]
public partial class DialogueSystem : Control
{
	[Export] public Dialogue CurrentDialogue;
	[Export] public RichTextLabel DialogueLabel;
	[Export] public float TransitionDuration = 0.3f;

	private int _currentLineIndex = -1;
	private bool _isActive = false;
	private Tween _tween;

	[Signal] public delegate void DialogueStartedEventHandler();
	[Signal] public delegate void DialogueLineChangedEventHandler(int lineIndex, string text);
	[Signal] public delegate void DialogueEndedEventHandler();

	public override void _Ready()
	{
		// Configurer le pivot au centre
		PivotOffset = Size / 2;
		
		// Commencer avec l'échelle à 0
		Scale = Vector2.Zero;
		Hide();
	}

	public override void _Process(double delta) { }

	public void StartDialogue()
	{
		if (CurrentDialogue == null || CurrentDialogue.DialogueTexts == null || CurrentDialogue.DialogueTexts.Count == 0)
		{
			GD.PushWarning("DialogueSystem: No dialogue to start.");
			return;
		}

		_currentLineIndex = -1;
		_isActive = true;
		
		// Afficher et animer l'entrée
		Show();
		AnimateIn();
		
		EmitSignal(SignalName.DialogueStarted);
		SwitchToNextLine();
	}

	public void SwitchToNextLine()
	{
		if (!_isActive) return;

		_currentLineIndex++;

		if (_currentLineIndex >= CurrentDialogue.DialogueTexts.Count)
		{
			EndDialogue();
			return;
		}

		var line = CurrentDialogue.DialogueTexts[_currentLineIndex];
		if (DialogueLabel != null)
			DialogueLabel.Text = line?.TextLine ?? string.Empty;

		EmitSignal(SignalName.DialogueLineChanged, _currentLineIndex, line?.TextLine ?? string.Empty);
	}

	public void EndDialogue()
	{
		_isActive = false;
		_currentLineIndex = -1;
		if (DialogueLabel != null)
			DialogueLabel.Text = string.Empty;

		// Animer la sortie puis cacher
		AnimateOut();
		
		EmitSignal(SignalName.DialogueEnded);
	}

	private void AnimateIn()
	{
		// Arrêter le tween précédent s'il existe
		if (_tween != null && _tween.IsValid())
		{
			_tween.Kill();
		}

		// Créer un nouveau tween
		_tween = CreateTween();
		_tween.SetEase(Tween.EaseType.Out);
		_tween.SetTrans(Tween.TransitionType.Back);
		
		// Animer de 0 à 1
		_tween.TweenProperty(this, "scale", Vector2.One, TransitionDuration)
			.From(Vector2.Zero);
	}

	private void AnimateOut()
	{
		// Arrêter le tween précédent s'il existe
		if (_tween != null && _tween.IsValid())
		{
			_tween.Kill();
		}

		// Créer un nouveau tween
		_tween = CreateTween();
		_tween.SetEase(Tween.EaseType.In);
		_tween.SetTrans(Tween.TransitionType.Back);
		
		// Animer de 1 à 0
		_tween.TweenProperty(this, "scale", Vector2.Zero, TransitionDuration)
			.From(Vector2.One);
		
		// Cacher après la fin de l'animation
		_tween.TweenCallback(Callable.From(Hide));
	}

	// Mettre à jour le pivot si la taille change
	public override void _Notification(int what)
	{
		if (what == NotificationResized)
		{
			PivotOffset = Size / 2;
		}
	}
}