using Godot;
using System;
namespace GameplayToolkit.InputAndControls
{
#if TOOLS
    [GlobalClass, Tool, Icon(IconPaths.TRIGGER)]
#endif
    public abstract partial class InputAnalogicAction : InputAction
    {
        [Export] public float DeadZone = 0.2f;
        [Export] public float DirtyThreshold = 0.01f;
    }
}