#if TOOLS
using Godot;

namespace GameplayToolkit.InputAndControls.Editor
{
	/// <summary>
	/// Popup qui écoute le prochain InputEvent (clavier, bouton manette, axe joystick)
	/// et l'émet via EventCaptured(actionIdx, fieldLabel, inputEvent).
	/// </summary>
	[Tool]
	public partial class InputEventCapturePopup : Window
	{
		[Signal]
		public delegate void EventCapturedEventHandler(int actionIdx, string fieldLabel, InputEvent inputEvent);

		private int _actionIdx;
		private string _fieldLabel;
		private Label _infoLabel;
		private bool _listening;

		// Seuil de deadzone pour les axes joystick
		private const float AxisThreshold = 0.5f;

		public override void _Ready()
		{
			Title = "Capturer un InputEvent";
			Size = new Vector2I(380, 180);
			Exclusive = true;
			Unresizable = true;

			var vbox = new VBoxContainer
			{
				AnchorsPreset = (int)Control.LayoutPreset.FullRect,
				OffsetLeft = 8,
				OffsetTop = 8,
				OffsetRight = -8,
				OffsetBottom = -8
			};
			AddChild(vbox);

			_infoLabel = new Label
			{
				Text = "Appuyez sur une touche, un bouton ou bougez un axe…",
				AutowrapMode = TextServer.AutowrapMode.WordSmart,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			vbox.AddChild(_infoLabel);
			vbox.AddChild(new HSeparator());

			var cancelBtn = new Button { Text = "Annuler" };
			cancelBtn.Pressed += () =>
			{
				_listening = false;
				Hide();
			};

			var hbox = new HBoxContainer();
			hbox.AddChild(new Control { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill }); // spacer
			hbox.AddChild(cancelBtn);
			vbox.AddChild(hbox);

			CloseRequested += () =>
			{
				_listening = false;
				Hide();
			};
		}

		/// <summary>Ouvre la popup en mode écoute.</summary>
		public void Open(int actionIdx, string fieldLabel)
		{
			_actionIdx = actionIdx;
			_fieldLabel = fieldLabel;
			_infoLabel.Text = $"Assignation de : [b]{fieldLabel}[/b]\nAppuyez sur une touche, bouton ou axe…";
			_infoLabel.UseParentMaterial = false;
			_listening = true;
			PopupCentered();
		}

		public override void _Input(InputEvent @event)
		{
			if (!_listening) return;
			if (!Visible) return;

			// ── Touche clavier ──────────────────────────────────────
			if (@event is InputEventKey key && key.Pressed && !key.Echo)
			{
				// Échappe = annuler
				if (key.Keycode == Key.Escape)
				{
					_listening = false;
					Hide();
					return;
				}

				var capture = new InputEventKey
				{
					Keycode = key.Keycode,
					Unicode = key.Unicode,
					Device = -1
				};
				Emit(capture);
				return;
			}

			// ── Bouton manette ──────────────────────────────────────
			if (@event is InputEventJoypadButton joyBtn && joyBtn.Pressed)
			{
				var capture = new InputEventJoypadButton
				{
					ButtonIndex = joyBtn.ButtonIndex,
					Device = joyBtn.Device
				};
				Emit(capture);
				return;
			}

			// ── Axe joystick (dépasse le seuil) ────────────────────
			if (@event is InputEventJoypadMotion joyAxis &&
				Mathf.Abs(joyAxis.AxisValue) > AxisThreshold)
			{
				var capture = new InputEventJoypadMotion
				{
					Axis = joyAxis.Axis,
					AxisValue = Mathf.Sign(joyAxis.AxisValue),
					Device = joyAxis.Device
				};
				Emit(capture);
			}
		}

		private void Emit(InputEvent ev)
		{
			_listening = false;
			EmitSignal(SignalName.EventCaptured, _actionIdx, _fieldLabel, ev);
			Hide();
		}
	}
}
#endif