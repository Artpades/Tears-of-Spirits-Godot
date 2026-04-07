using Godot;
using System;
namespace GameplayToolkit.InputAndControls
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.INPUTACTIONAXIS)]
#endif
	public partial class InputAxisAction : InputAnalogicAction
	{
		[Signal] public delegate void AxisEmittedEventHandler(float value);

		[ExportGroup("⌨️ Key Axis")]
		[Export] public Key NegativeKey;
		[Export] public Key PositiveKey;

		[ExportGroup("🔘 Joy Axis")]
		[Export] public JoyAxis JoyAxis = JoyAxis.Invalid;

		[ExportGroup("🔲 Joy Button")]
		[Export] public JoyButton NegativeButton = JoyButton.Invalid;
		[Export] public JoyButton PositiveButton = JoyButton.Invalid;

		private float _lastEmittedValue = 0f;

		public override void _Process(double delta)
		{
			float current = GetFloatAxis();
			if (Mathf.Abs(current - _lastEmittedValue) < DirtyThreshold) return;
			_lastEmittedValue = current;
			EmitSignal(SignalName.AxisEmitted, current);

			if (DebugTools.LogInputActions)
				CustomLogger.Log(Name, IconPaths.INPUTACTIONAXIS,
					$"[color=cyan]AxisEmitted[/color] → [b]{current:F3}[/b]");
		}
		public float GetFloatAxis()
		{
			// Joystick en priorité
			if (ControllerAllowed && JoyAxis != JoyAxis.Invalid)
			{
				float joy = Input.GetJoyAxis(DeviceID, JoyAxis);
				if (Mathf.Abs(joy) > DeadZone)
					return Mathf.Clamp(joy, -1f, 1f);
			}

			float value = 0f;

			if (KeyboardAllowed)
			{
				if (PositiveKey != Key.None && Input.IsKeyPressed(PositiveKey)) value += 1f;
				if (NegativeKey != Key.None && Input.IsKeyPressed(NegativeKey)) value -= 1f;
			}

			if (ControllerAllowed)
			{
				if (PositiveButton != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, PositiveButton)) value += 1f;
				if (NegativeButton != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, NegativeButton)) value -= 1f;
			}

			return Mathf.Clamp(value, -1f, 1f);
		}
	}
}