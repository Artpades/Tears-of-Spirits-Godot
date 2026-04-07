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

	private int _currentLineIndex = -1;
	private bool _isActive = false;

	[Signal] public delegate void DialogueStartedEventHandler();
	[Signal] public delegate void DialogueLineChangedEventHandler(int lineIndex, string text);
	[Signal] public delegate void DialogueEndedEventHandler();

	public override void _Ready()
	{
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
		Show();
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

		Hide();
		EmitSignal(SignalName.DialogueEnded);
	}
}