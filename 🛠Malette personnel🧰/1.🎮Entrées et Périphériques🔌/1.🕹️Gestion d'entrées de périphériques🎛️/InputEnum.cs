using Godot;
using System;

namespace GameplayToolkit.InputAndControls
{
	public enum DeviceDetectorType
	{
		AllDevice,
		KeyboardOnly,
		ControllersOnly
	}
	public enum KeyAxisType
	{
		WASD,
		ZQSD,
		ArrowKeys,
		Custom
	}
	public enum JoyAxisType
	{
		LeftStick,
		RightStick,
		Custom
	}
	public enum DpadType
	{
		TraditionalDpad,
		ABXY,
		Custom
	}
}