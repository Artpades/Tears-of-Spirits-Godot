using Godot;
using System;
namespace GameplayToolkit.Cameras;

[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.CAMEFFECT)
]
public abstract partial class CameraEffect : Resource
{
    public VirtualCamera3D TargetCam3D;
    public VirtualCamera2D TargetCam2D;
    public abstract void ProcessEffect(float delta);
}
