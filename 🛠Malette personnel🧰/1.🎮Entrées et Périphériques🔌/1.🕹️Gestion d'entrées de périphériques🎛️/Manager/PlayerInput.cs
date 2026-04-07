using Godot;
using System;
namespace GameplayToolkit.InputAndControls
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.INPUTMANAGER)]
#endif
	public partial class PlayerInput : Node
	{
		private int _deviceNumber;
		private DeviceDetectorType _detectorType;

		[Export]
		public int DeviceNumber
		{
			get => _deviceNumber;
			set
			{
				if (_deviceNumber == value) return;
				_deviceNumber = value;
				PropagateToActions();

				if (DebugTools.LogInputManagers)
					CustomLogger.Log(Name, IconPaths.INPUTMANAGER,
						$"[color=yellow]DeviceNumber[/color] changé → [b]{_deviceNumber}[/b]");
			}
		}

		[Export]
		public DeviceDetectorType DetectorType
		{
			get => _detectorType;
			set
			{
				if (_detectorType == value) return;
				_detectorType = value;
				PropagateToActions();

				if (DebugTools.LogInputManagers)
					CustomLogger.Log(Name, IconPaths.INPUTMANAGER,
						$"[color=yellow]DetectorType[/color] changé → [b]{_detectorType}[/b]");
			}
		}
		[Export] public bool ChangeDetectorOnDeviceInput;

		public override void _Ready()
		{
			// CallDeferred garantit que tous les enfants sont prêts
			CallDeferred(MethodName.PropagateToActions);

			if (DebugTools.LogInputManagers)
				CustomLogger.Log(Name, IconPaths.INPUTMANAGER,
					$"[color=green]Prêt[/color] — Device [b]{_deviceNumber}[/b], Filter [b]{_detectorType}[/b]");

		}

		[Export] private float JoyInputThreshold = 0.2f;

		public override void _Input(InputEvent @event)
		{
			if (!ChangeDetectorOnDeviceInput) return;

			if (@event is InputEventKey)
			{
				DetectorType = DeviceDetectorType.KeyboardOnly;
			}
			else if (@event is InputEventJoypadButton || @event is InputEventJoypadMotion motion && Mathf.Abs(motion.AxisValue) > JoyInputThreshold)
			{
				DetectorType = DeviceDetectorType.ControllersOnly;
			}
		}

		public void StartVibration(float weakMagnitude = 1, float strongMagnitude = 1, float duration = 0)
		{
			Input.StartJoyVibration(DeviceNumber, weakMagnitude, strongMagnitude, duration);
		}
		public void StopJoyVibration()
		{
			Input.StopJoyVibration(DeviceNumber);
		}

		private void PropagateToActions()
		{
			foreach (var action in TypeSearcher.GetAllChildrenOfType<InputAction>(this))
			{
				action.DeviceID = _deviceNumber;
				action.ActiveDeviceFilter = _detectorType;
			}

			if (DebugTools.LogInputManagers)
				CustomLogger.Log(Name, IconPaths.INPUTMANAGER,
					$"[color=orange]Propagation[/color] → [b]{TypeSearcher.GetAllChildrenOfType<InputAction>(this).Count}[/b] action(s) mise(s) à jour");
		}

	}


}