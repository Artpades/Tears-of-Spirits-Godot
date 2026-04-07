using Godot;
using System;

namespace GameplayToolkit.InputAndControls
{
#if TOOLS
    [GlobalClass, Tool, Icon(IconPaths.INPUTACTION)]
#endif
    public abstract partial class InputAction : Node
    {
        public string ActionName => Name;
        [Export] public int DeviceID;

        [Export] public DeviceDetectorType ActiveDeviceFilter { get; set; } = DeviceDetectorType.AllDevice;

        protected bool KeyboardAllowed => ActiveDeviceFilter is DeviceDetectorType.AllDevice or DeviceDetectorType.KeyboardOnly;
        protected bool ControllerAllowed => ActiveDeviceFilter is DeviceDetectorType.AllDevice or DeviceDetectorType.ControllersOnly;
    }

}