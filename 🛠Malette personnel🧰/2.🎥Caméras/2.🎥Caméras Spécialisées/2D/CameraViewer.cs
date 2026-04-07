using Godot;
using Godot.Collections;
using System;

namespace GameplayToolkit.Cameras
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.CANVA)]
#endif
	public partial class CameraViewer : VirtualCamera2D
	{
		// ── Zoom ─────────────────────────────────────────────────────────────────
		[ExportCategory("Settings")]
		[Export] public float ZoomMin = 0.2f;
		[Export] public float ZoomMax = 5.0f;
		[Export] public float ZoomStep = 0.1f;   // pas par cran de molette
		[Export] public float ZoomSmoothing = 8.0f;   // 0 = instantané

		// ── Souris ────────────────────────────────────────────────────────────────
		[ExportGroup("Mouse Settings")]
		[Export] public MouseButton PanButton = MouseButton.Middle;
		// ── Manette ───────────────────────────────────────────────────────────────
		[ExportGroup("Gamepad Settings")]
		[Export] public float GamepadPanSpeed = 400.0f;
		[Export] public float GamepadZoomSpeed = 1.5f;
		[Export] public float GamepadDeadzone = 0.15f;

		// ─────────────────────────────────────────────────────────────────────────
		// État interne
		// ─────────────────────────────────────────────────────────────────────────

		private float _targetZoom = 1.0f;

		// Souris
		private bool _mouseIsPanning = false;
		private Vector2 _mousePanOrigin = Vector2.Zero;
		private Vector2 _cameraPanOrigin = Vector2.Zero;

		// Tactile : index → position courante en pixels écran
		private readonly Dictionary<int, Vector2> _touches = [];

		// État capturé au début d'un pinch
		private float _pinchStartDist = 0f;
		private float _pinchStartZoom = 1f;
		private Vector2 _pinchStartMid = Vector2.Zero;
		private Vector2 _pinchCamOrigin = Vector2.Zero;

		// ─────────────────────────────────────────────────────────────────────────
		public override void _Ready()
		{
			_targetZoom = Zoom.X;
		}

		// ─────────────────────────────────────────────────────────────────────────
		public override void _Process(double delta)
		{
			ProcessGamepad((float)delta);
			SmoothZoom((float)delta);
		}

		// ─────────────────────────────────────────────────────────────────────────
		public override void _Input(InputEvent @event)
		{
			switch (@event)
			{
				case InputEventMouseButton mb:
					OnMouseButton(mb);
					break;

				case InputEventMouseMotion mm when _mouseIsPanning:
					Position = _cameraPanOrigin - (mm.Position - _mousePanOrigin) / Zoom.X;
					break;

				case InputEventScreenTouch touch:
					OnScreenTouch(touch);
					break;

				case InputEventScreenDrag drag:
					OnScreenDrag(drag);
					break;
			}
		}

		// =========================================================================
		// Souris
		// =========================================================================

		private void OnMouseButton(InputEventMouseButton mb)
		{
			if (mb.ButtonIndex == MouseButton.WheelUp)
			{
				ZoomTowardPoint(mb.Position, ZoomStep);
				return;
			}
			if (mb.ButtonIndex == MouseButton.WheelDown)
			{
				ZoomTowardPoint(mb.Position, -ZoomStep);
				return;
			}

			if (mb.ButtonIndex == PanButton)
			{
				_mouseIsPanning = mb.Pressed;
				if (_mouseIsPanning)
				{
					_mousePanOrigin = mb.Position;
					_cameraPanOrigin = Position;
				}
			}
		}

		/// <summary>
		/// Zoom centré sur un point écran : le point sous le curseur reste fixe.
		/// </summary>
		private void ZoomTowardPoint(Vector2 screenPoint, float step)
		{
			float oldZoom = _targetZoom;
			_targetZoom = Mathf.Clamp(oldZoom + step, ZoomMin, ZoomMax);

			// worldPoint = (screenPoint - vpCenter) / oldZoom + cameraPos
			Vector2 vpCenter = GetViewport().GetVisibleRect().Size * 0.5f;
			Vector2 worldPt = (screenPoint - vpCenter) / oldZoom + Position;
			Position = worldPt - (screenPoint - vpCenter) / _targetZoom;

			// Met à jour l'origine du pan souris si actif
			_cameraPanOrigin = Position;
		}

		// =========================================================================
		// Manette
		// =========================================================================

		private void ProcessGamepad(float delta)
		{
			// Pan – joystick gauche
			float lx = ApplyDeadzone(Input.GetJoyAxis(0, JoyAxis.RightX));
			float ly = ApplyDeadzone(Input.GetJoyAxis(0, JoyAxis.RightY));

			if (lx != 0f || ly != 0f)
				Position += new Vector2(lx, ly) * GamepadPanSpeed * delta / Zoom.X;

			// Zoom – R2 (zoom in) / L2 (zoom out)
			float zDelta = (Input.GetJoyAxis(0, JoyAxis.TriggerRight)
						  - Input.GetJoyAxis(0, JoyAxis.TriggerLeft))
						  * GamepadZoomSpeed * delta;

			if (Mathf.Abs(zDelta) > 0.001f)
				_targetZoom = Mathf.Clamp(_targetZoom + zDelta, ZoomMin, ZoomMax);
		}

		private float ApplyDeadzone(float value) => Mathf.Abs(value) < GamepadDeadzone ? 0f : value;

		// =========================================================================
		// Tactile
		// =========================================================================

		private void OnScreenTouch(InputEventScreenTouch touch)
		{
			if (touch.Pressed)
				_touches[touch.Index] = touch.Position;
			else
				_touches.Remove(touch.Index);

			if (_touches.Count == 2)
				CapturePinchState();
		}

		private void OnScreenDrag(InputEventScreenDrag drag)
		{
			_touches[drag.Index] = drag.Position;

			if (_touches.Count == 1)
			{
				// Pan 1 doigt
				Position -= drag.Relative / Zoom.X;
			}
			else if (_touches.Count == 2)
			{
				var keys = new Array<int>(_touches.Keys);
				Vector2 posA = _touches[keys[0]];
				Vector2 posB = _touches[keys[1]];

				// Pinch zoom
				if (_pinchStartDist > 0f)
				{
					float ratio = posA.DistanceTo(posB) / _pinchStartDist;
					_targetZoom = Mathf.Clamp(_pinchStartZoom * ratio, ZoomMin, ZoomMax);
					Zoom = Vector2.One * _targetZoom; // appliqué immédiatement pour que le pan soit cohérent
				}

				// Pan 2 doigts (milieu des deux doigts)
				Vector2 midDelta = ((posA + posB) * 0.5f - _pinchStartMid) / Zoom.X;
				Position = _pinchCamOrigin - midDelta;
			}
		}

		private void CapturePinchState()
		{
			var keys = new Array<int>(_touches.Keys);
			Vector2 posA = _touches[keys[0]];
			Vector2 posB = _touches[keys[1]];

			_pinchStartDist = posA.DistanceTo(posB);
			_pinchStartZoom = Zoom.X;
			_pinchStartMid = (posA + posB) * 0.5f;
			_pinchCamOrigin = Position;
		}

		// =========================================================================
		// Zoom lissé
		// =========================================================================

		private void SmoothZoom(float delta)
		{
			float next = ZoomSmoothing > 0f
				? Mathf.Lerp(Zoom.X, _targetZoom, ZoomSmoothing * delta)
				: _targetZoom;
			Zoom = Vector2.One * next;
		}
	}
}