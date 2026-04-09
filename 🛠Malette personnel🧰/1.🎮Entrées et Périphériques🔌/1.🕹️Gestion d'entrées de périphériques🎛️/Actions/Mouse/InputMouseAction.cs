using Godot;
using System;
namespace GameplayToolkit.InputAndControls
{
#if TOOLS
    [GlobalClass, Tool, Icon(IconPaths.INPUTACTIONMOUSE)]
#endif
    public partial class InputMouseAction : InputAction
    {
        [Signal] public delegate void PositionEmittedEventHandler(Vector2 position);
        [Signal] public delegate void MotionEmittedEventHandler(Vector2 motion);
        [Signal] public delegate void PressureChangedEventHandler(float pressure);

        [ExportGroup("⚙️ Paramètres")]
        [Export(PropertyHint.Range, "0.1,5.0,0.1")]
        public float MotionSensitivity { get; set; } = 1.0f;

        [Export(PropertyHint.Range, "0,10,0.1")]
        public float MotionThreshold { get; set; } = 0.5f;

        [Export]
        public bool NormalizePosition { get; set; } = false;

        private Vector2 _previousPosition = Vector2.Zero;
        private Vector2 _accumulatedMotion = Vector2.Zero;
        private float _currentPressure = 0f;
        private float _previousPressure = 0f;
        private bool _hasViewport = false;
        private bool _mouseInsideViewport = false; // ← AJOUT
        private Viewport _viewport;

        public override void _Ready()
        {
            SetViewport(GetViewport());
        }

        public override void _Notification(int what) // ← AJOUT
        {
            if (what == NotificationWMMouseEnter)
            {
                _mouseInsideViewport = true;
            }
            else if (what == NotificationWMMouseExit)
            {
                _mouseInsideViewport = false;
                // Réinitialiser tout proprement à la sortie
                _accumulatedMotion = Vector2.Zero;
                _currentPressure = 0f;
            }
        }

        public override void _Process(double delta)
        {
            if (Engine.IsEditorHint()) return;
            if (!_hasViewport) return;
            if (!_mouseInsideViewport)
            {
                if (!_previousPosition.IsEqualApprox(Vector2.Zero))
                {
                    EmitSignal(SignalName.MotionEmitted, Vector2.Zero);
                    if (!Mathf.IsEqualApprox(_previousPressure, 0f))
                    {
                        EmitSignal(SignalName.PressureChanged, 0f);
                        _previousPressure = 0f;
                    }
                    _previousPosition = Vector2.Zero;
                }
                return;
            }

            Vector2 currentPosition = GetCurrentPosition();
            Vector2 currentMotion = _accumulatedMotion * MotionSensitivity;

            // Position : émettre seulement si elle a changé
            if (!currentPosition.IsEqualApprox(_previousPosition))
            {
                EmitSignal(SignalName.PositionEmitted, currentPosition);
                _previousPosition = currentPosition;
            }

            // Mouvement : émettre seulement si le delta dépasse le seuil
            if (currentMotion.Length() > MotionThreshold)
            {
                EmitSignal(SignalName.MotionEmitted, currentMotion);
            }
            else if (!_accumulatedMotion.IsZeroApprox()) // ← CHANGEMENT ICI
            {
                EmitSignal(SignalName.MotionEmitted, Vector2.Zero);
            }

            // Pression : émettre seulement si elle a changé
            if (!Mathf.IsEqualApprox(_currentPressure, _previousPressure))
            {
                EmitSignal(SignalName.PressureChanged, _currentPressure);
                _previousPressure = _currentPressure;
            }

            _accumulatedMotion = Vector2.Zero;
        }

        public override void _Input(InputEvent @event)
        {
            if (Engine.IsEditorHint() && ProcessMode == ProcessModeEnum.Disabled) return;
            if (!_mouseInsideViewport) return; // ← AJOUT : ignorer les events hors viewport

            if (@event is InputEventMouseMotion motionEvent)
            {
                AccumulateMotion(motionEvent);
            }
        }

        public void SetViewport(Viewport viewport)
        {
            _viewport = viewport;
            _hasViewport = viewport != null;
            if (_hasViewport)
                _previousPosition = GetCurrentPosition();
        }

        public void AccumulateMotion(InputEventMouseMotion motionEvent)
        {
            if (motionEvent == null) return;
            _accumulatedMotion += motionEvent.Relative;
            _currentPressure = motionEvent.Pressure;
        }

        public void AccumulateMotion(Vector2 relativeMotion)
        {
            _accumulatedMotion += relativeMotion;
        }

        private Vector2 GetCurrentPosition()
        {
            if (!_hasViewport) return Vector2.Zero;

            Vector2 mousePos = _viewport.GetMousePosition();

            if (NormalizePosition)
            {
                Vector2 viewportSize = _viewport.GetVisibleRect().Size;
                if (viewportSize.X > 0 && viewportSize.Y > 0)
                    mousePos = new Vector2(mousePos.X / viewportSize.X, mousePos.Y / viewportSize.Y);
            }

            return mousePos;
        }

        public Vector2 GetPosition() => _mouseInsideViewport ? GetCurrentPosition() : Vector2.Zero; // ← AJOUT
        public Vector2 GetAccumulatedMotion() => _mouseInsideViewport ? _accumulatedMotion * MotionSensitivity : Vector2.Zero; // ← AJOUT
        public float GetPressure() => _mouseInsideViewport ? _currentPressure : 0f; // ← AJOUT
    }
}