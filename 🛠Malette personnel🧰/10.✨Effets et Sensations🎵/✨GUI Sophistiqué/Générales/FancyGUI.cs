using Godot;
using Godot.Collections;
using System;
namespace GameplayToolkit.VisualAndSoundEffects.GUI;
#if TOOLS
[GlobalClass, Tool, Icon(IconPaths.FANCYGUI)]
#endif
public partial class FancyGUI : Node
{
	public bool isInEditorMode;
	private Control FocusParent => GetParent<Control>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!isInEditorMode)
		{
			if (Engine.IsEditorHint()) return;
			{
				FocusParent.PivotOffset = FocusParent.Size / 2;
				InitMods();
			}
		}
		else if (isInEditorMode)
		{
			FocusParent.PivotOffset = FocusParent.Size / 2;
			InitMods();
		}
	}
	internal void InitMods()
	{
		if (SizeChangeActivated) InitSizeChangeMode();
		HandleSound();
		InitializeFancyTooltip();

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ProcessTooltip();
	}
}
