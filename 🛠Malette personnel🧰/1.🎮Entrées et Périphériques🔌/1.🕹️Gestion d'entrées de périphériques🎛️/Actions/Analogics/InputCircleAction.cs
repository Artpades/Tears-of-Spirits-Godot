using Godot;
using System;
namespace GameplayToolkit.InputAndControls
{
#if TOOLS
    [GlobalClass, Tool, Icon(IconPaths.INPUTACTIONCIRCLE)]
#endif
    public partial class InputCircleAction : InputAnalogicAction
    {
        [Signal] public delegate void CircleEmittedEventHandler(Vector2 value);

        [ExportGroup("⌨️ Key Axis"), Export]
        public KeyAxisType KeyAxisPrefabType
        {
            get => _currentKeyAxisType;
            set
            {
                _currentKeyAxisType = value;
                switch (value)
                {
                    case KeyAxisType.WASD:
                        UpKey = Key.W;
                        DownKey = Key.S;
                        LeftKey = Key.A;
                        RightKey = Key.D;
                        break;
                    case KeyAxisType.ZQSD:
                        UpKey = Key.Z;
                        DownKey = Key.S;
                        LeftKey = Key.Q;
                        RightKey = Key.D;
                        break;
                    case KeyAxisType.ArrowKeys:
                        UpKey = Key.Up;
                        DownKey = Key.Down;
                        LeftKey = Key.Left;
                        RightKey = Key.Right;
                        break;
                    case KeyAxisType.Custom:
                        // Ne rien faire, l'utilisateur doit assigner manuellement les touches
                        break;
                }
            }
        }

        [Export] public Key LeftKey;
        [Export] public Key RightKey;
        [Export] public Key UpKey;
        [Export] public Key DownKey;
        private KeyAxisType _currentKeyAxisType = KeyAxisType.Custom;



        [ExportGroup("🔘 Joy Axis"), Export]
        public JoyAxisType JoyStickPrefabType
        {
            get => _currentJoyAxisType;
            set
            {
                _currentJoyAxisType = value;
                switch (value)
                {
                    case JoyAxisType.LeftStick:
                        JoyAxisX = JoyAxis.LeftX;
                        JoyAxisY = JoyAxis.LeftY;
                        break;
                    case JoyAxisType.RightStick:
                        JoyAxisX = JoyAxis.RightX;
                        JoyAxisY = JoyAxis.RightY;
                        break;
                    case JoyAxisType.Custom:
                        // Ne rien faire, l'utilisateur doit assigner manuellement les axes
                        break;
                }
            }
        }

        private JoyAxisType _currentJoyAxisType = JoyAxisType.Custom;
        [Export] public JoyAxis JoyAxisX = JoyAxis.Invalid;
        [Export] public JoyAxis JoyAxisY = JoyAxis.Invalid;



        [ExportGroup("✖️DPAD Axis"), Export]
        public DpadType DpadPrefabType
        {
            get => _currentDpadType;
            set
            {
                _currentDpadType = value;
                switch (value)
                {
                    case DpadType.TraditionalDpad:
                        DpadUp = JoyButton.DpadUp;
                        DpadDown = JoyButton.DpadDown;
                        DpadLeft = JoyButton.DpadLeft;
                        DpadRight = JoyButton.DpadRight;
                        break;
                    case DpadType.ABXY:
                        DpadUp = JoyButton.Y;
                        DpadDown = JoyButton.A;
                        DpadLeft = JoyButton.X;
                        DpadRight = JoyButton.B;
                        break;
                    case DpadType.Custom:
                        // Ne rien faire, l'utilisateur doit assigner manuellement les boutons
                        break;
                }
            }
        }
        [Export] public JoyButton DpadLeft = JoyButton.Invalid;
        [Export] public JoyButton DpadRight = JoyButton.Invalid;
        [Export] public JoyButton DpadUp = JoyButton.Invalid;
        [Export] public JoyButton DpadDown = JoyButton.Invalid;

        private DpadType _currentDpadType = DpadType.Custom;


        private Vector2 _lastEmittedValue = Vector2.Zero;

        public override void _Process(double delta)
        {
            if(Engine.IsEditorHint()) return;
            Vector2 current = GetVector2Axis();
            if (current.DistanceSquaredTo(_lastEmittedValue) < DirtyThreshold * DirtyThreshold) return;
            _lastEmittedValue = current;
            EmitSignal(SignalName.CircleEmitted, current);

            if (DebugTools.LogInputActions)
                CustomLogger.Log(Name, IconPaths.INPUTACTIONCIRCLE,
                    $"[color=cyan]CircleEmitted[/color] → [b]({current.X:F3}, {current.Y:F3})[/b]");
        }


        public Vector2 GetVector2Axis()
        {
            Vector2 joyValue = Vector2.Zero;
            Vector2 buttonValue = Vector2.Zero;

            if (ControllerAllowed && JoyAxisX != JoyAxis.Invalid && JoyAxisY != JoyAxis.Invalid)
            {
                Vector2 joy = new(
                    Input.GetJoyAxis(DeviceID, JoyAxisX),
                    Input.GetJoyAxis(DeviceID, JoyAxisY)
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
                if (DownKey != Key.None && Input.IsKeyPressed(DownKey)) buttonValue.Y += 1f;
                if (UpKey != Key.None && Input.IsKeyPressed(UpKey)) buttonValue.Y -= 1f;
            }

            if (ControllerAllowed && !joystickActive)
            {
                if (DpadRight != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, DpadRight)) buttonValue.X += 1f;
                if (DpadLeft != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, DpadLeft)) buttonValue.X -= 1f;
                if (DpadDown != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, DpadDown)) buttonValue.Y += 1f;
                if (DpadUp != JoyButton.Invalid && Input.IsJoyButtonPressed(DeviceID, DpadUp)) buttonValue.Y -= 1f;
            }

            if (buttonValue.LengthSquared() > 1f)
                buttonValue = buttonValue.Normalized();


            return joyValue.LengthSquared() > 0f ? joyValue : buttonValue;
        }
    }
}