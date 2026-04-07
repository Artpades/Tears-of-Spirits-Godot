#if TOOLS
using Godot;

namespace GameplayToolkit.ToolsAndEdit
{
	[Tool]
	public partial class CameraPannerEditor : Camera2D
	{
		public Control ControlForPanning;
		public MouseButton PanMouseButton { get; set; } = MouseButton.Middle;
		public float ZoomFactor { get; set; } = 1.15f;
		public float MinZoom { get; set; } = 0.1f;
		public float MaxZoom { get; set; } = 10f;
		public float LerpSpeed { get; set; } = 20f;

		private Vector2 _targetPosition;
		private float _targetZoom = 1f;

		public override void _Ready()
		{
			_targetPosition = Position;
			_targetZoom = Zoom.X;
		}

		public override void _Process(double delta)
		{
			Position = Position.Lerp(_targetPosition, LerpSpeed * (float)delta);
			float smoothZoom = Mathf.Lerp(Zoom.X, _targetZoom, LerpSpeed * (float)delta);
			Zoom = new Vector2(smoothZoom, smoothZoom);
			
		}

		public override void _Input(InputEvent @event)
		{
			// Panning : uniquement quand la souris se déplace avec le bon bouton enfoncé
			if (@event is InputEventMouseMotion motionEvent && Input.IsMouseButtonPressed(PanMouseButton))
			{
				// Relative = déplacement depuis le frame précédent, pas de vélocité obsolète
				_targetPosition -= motionEvent.Relative / _targetZoom;
			}

			if (@event is not InputEventMouseButton mouseButtonEvent) return;
			if (!mouseButtonEvent.Pressed) return;

			switch (mouseButtonEvent.ButtonIndex)
			{
				case MouseButton.WheelUp:
					ApplyZoom(ZoomFactor, mouseButtonEvent.Position);
					break;
				case MouseButton.WheelDown:
					ApplyZoom(1f / ZoomFactor, mouseButtonEvent.Position);
					break;
			}
		}

		private void ApplyZoom(float factor, Vector2 mouseScreenPos)
		{
			float newZoom = Mathf.Clamp(_targetZoom * factor, MinZoom, MaxZoom);

			Vector2 screenCenter = GetViewportRect().Size / 2f;
			Vector2 offsetScreen = mouseScreenPos - screenCenter;
			Vector2 worldBefore = _targetPosition + offsetScreen / _targetZoom;
			Vector2 worldAfter = _targetPosition + offsetScreen / newZoom;
			_targetPosition += worldBefore - worldAfter;

			_targetZoom = newZoom;
		}
	}
}
#endif