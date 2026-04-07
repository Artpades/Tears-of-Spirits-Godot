using Godot;
using Godot.Collections;
using System;
namespace GameplayToolkit.InputAndControls
{
#if TOOLS
    [GlobalClass, Tool, Icon(IconPaths.INPUTACTIONBUTTON)]
#endif
    public partial class InputButtonAction : InputAction
    {
        [Signal] public delegate void ButtonPressedEventHandler();
        [Signal] public delegate void ButtonHeldEventHandler();
        [Signal] public delegate void ButtonReleasedEventHandler();
        [ExportGroup("⌨️ Key Button")]
        [Export] public Key Key;
        [ExportGroup("🖱️ Mouse Button")]
        [Export] public MouseButton MouseButton;
        [ExportGroup("🔘 Joy Button")]
        [Export] public JoyButton JoyButton = JoyButton.Invalid;
        public bool IsPressed { get; private set; }
        private bool _wasButtonPressed;
        public override void _Ready()
        {
        }
        public override void _Process(double delta)
        {
            bool isPressed = IsPressed = IsButtonPressed();
            if (isPressed && !_wasButtonPressed)
            {
                _wasButtonPressed = true;
                EmitSignal(SignalName.ButtonPressed);

                if (DebugTools.LogInputActions)
                    CustomLogger.Log(Name, IconPaths.INPUTACTIONBUTTON,
                        $"[color=green]ButtonPressed[/color] — Key [b]{Key}[/b] / Joy [b]{JoyButton}[/b] / Mouse [b]{MouseButton}[/b]");
            }
            else if (isPressed && _wasButtonPressed)
            {
                EmitSignal(SignalName.ButtonHeld);

                if (DebugTools.LogInputActions)
                    CustomLogger.Log(Name, IconPaths.INPUTACTIONBUTTON,
                        $"[color=yellow]ButtonHeld[/color] — Key [b]{Key}[/b] / Joy [b]{JoyButton}[/b] / Mouse [b]{MouseButton}[/b]");
            }
            else if (_wasButtonPressed)
            {
                _wasButtonPressed = false;
                EmitSignal(SignalName.ButtonReleased);

                if (DebugTools.LogInputActions)
                    CustomLogger.Log(Name, IconPaths.INPUTACTIONBUTTON,
                        $"[color=red]ButtonReleased[/color] — Key [b]{Key}[/b] / Joy [b]{JoyButton}[/b] / Mouse [b]{MouseButton}[/b]");
            }
        }

        private bool IsButtonPressed()
        {
            if (KeyboardAllowed && Key != Key.None && Input.IsKeyPressed(Key)) return true;
            if (KeyboardAllowed && MouseButton != MouseButton.None && Input.IsMouseButtonPressed(MouseButton)) return true;

            if (ControllerAllowed && JoyButton != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, JoyButton)) return true;

            return false;
        }
    }
}