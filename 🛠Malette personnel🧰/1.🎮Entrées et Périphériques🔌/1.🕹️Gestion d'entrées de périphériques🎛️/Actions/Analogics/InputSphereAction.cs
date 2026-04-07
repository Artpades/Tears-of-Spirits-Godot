using Godot;
using System;
namespace GameplayToolkit.InputAndControls
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.INPUTACTIONSPHERE)]
#endif
	public partial class InputSphereAction : InputAnalogicAction
	{
		[Signal] public delegate void SphereEmittedEventHandler(Vector3 vector);

		[ExportGroup("⌨️ Key Axis")]
		[Export] public Key LeftKey;
		[Export] public Key RightKey;
		[Export] public Key UpKey;
		[Export] public Key DownKey;
		[Export] public Key ForwardKey;
		[Export] public Key BackwardKey;

		[ExportGroup("🔘 Joy Axis")]
		[Export] public JoyAxis JoyAxisX = JoyAxis.Invalid;
		[Export] public JoyAxis JoyAxisY = JoyAxis.Invalid;
		[Export] public JoyAxis JoyAxisZ = JoyAxis.Invalid;

		[ExportGroup("✖️DPAD Axis")]
		[Export] public JoyButton DpadLeft = JoyButton.Invalid;
		[Export] public JoyButton DpadRight = JoyButton.Invalid;
		[Export] public JoyButton DpadUp = JoyButton.Invalid;
		[Export] public JoyButton DpadDown = JoyButton.Invalid;
		[Export] public JoyButton DpadForward = JoyButton.Invalid;
		[Export] public JoyButton DpadBackward = JoyButton.Invalid;

		private Vector3 _lastEmittedValue = Vector3.Zero;


		public override void _Process(double delta)
		{
			Vector3 current = GetVector3Axis();
			if (current.DistanceSquaredTo(_lastEmittedValue) < DirtyThreshold * DirtyThreshold) return;
			_lastEmittedValue = current;
			EmitSignal(SignalName.SphereEmitted, current);

			if (DebugTools.LogInputActions)
				CustomLogger.Log(Name, IconPaths.INPUTACTIONSPHERE,
					$"[color=cyan]SphereEmitted[/color] → [b]({current.X:F3}, {current.Y:F3}, {current.Z:F3})[/b]");
		}


		public Vector3 GetVector3Axis()
		{
			Vector3 joyValue = Vector3.Zero;
			Vector3 buttonValue = Vector3.Zero;

			if (ControllerAllowed && JoyAxisX != JoyAxis.Invalid && JoyAxisY != JoyAxis.Invalid && JoyAxisZ != JoyAxis.Invalid)
			{
				Vector3 joy = new(
					Input.GetJoyAxis(DeviceID, JoyAxisX),
					Input.GetJoyAxis(DeviceID, JoyAxisY),
					Input.GetJoyAxis(DeviceID, JoyAxisZ)
				);
				if (joy.Length() > DeadZone)
					joyValue = joy;
			}

			// Le joystick a la priorité — on lit les boutons seulement si pas de joystick actif
			bool joystickActive = joyValue.LengthSquared() > 0f;

			if (KeyboardAllowed && !joystickActive)
			{
				if (RightKey != Key.None && Input.IsKeyPressed(RightKey)) buttonValue.X += 1f;
				if (LeftKey != Key.None && Input.IsKeyPressed(LeftKey)) buttonValue.X -= 1f;
				if (UpKey != Key.None && Input.IsKeyPressed(UpKey)) buttonValue.Y += 1f;
				if (DownKey != Key.None && Input.IsKeyPressed(DownKey)) buttonValue.Y -= 1f;
				if (ForwardKey != Key.None && Input.IsKeyPressed(ForwardKey)) buttonValue.Z += 1f;
				if (BackwardKey != Key.None && Input.IsKeyPressed(BackwardKey)) buttonValue.Z -= 1f;
			}

			if (ControllerAllowed && !joystickActive)
			{
				if (DpadRight != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, DpadRight)) buttonValue.X += 1f;
				if (DpadLeft != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, DpadLeft)) buttonValue.X -= 1f;
				if (DpadUp != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, DpadUp)) buttonValue.Y += 1f;
				if (DpadDown != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, DpadDown)) buttonValue.Y -= 1f;
				if (DpadForward != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, DpadForward)) buttonValue.Z += 1f;
				if (DpadBackward != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, DpadBackward)) buttonValue.Z -= 1f;
			}

			if (buttonValue.LengthSquared() > 1f)
				buttonValue = buttonValue.Normalized();


			return joyValue.LengthSquared() > 0f ? joyValue : buttonValue;
		}
	}
}