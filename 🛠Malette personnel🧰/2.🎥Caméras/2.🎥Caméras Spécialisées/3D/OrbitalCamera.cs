using Godot;
using System;
namespace GameplayToolkit.Cameras;

[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.ORBITALCAMERA)
]
public partial class OrbitalCamera : VirtualCamera3D
{
    [Export] public Node3D TargetNode;
    /// <summary> La référence permettant à la caméra de ciblé l'objet </summary>
    [Export] public Node3D NodeParentOfCam; // à rendre parent de arm3D

    /// <summary>Référenrece d'un SpringArm permettant de faire tourner la caméra</summary>
	[Export] public SpringArm3D Arm3D; // à rendre parent de OrbitalCam

    [Export] public float Sensitivity = 0.005f;
    public override void _Ready()
    {
        base._Ready();
        //CallDeferred(nameof(InitializeCam));
    }
    private Vector3 _inputRot;

    public override void _Process(double delta)
    {
        base._Process(delta);

        Arm3D.Rotation += _inputRot * Sensitivity;

        // Clamp du pitch après application
        Arm3D.Rotation = new Vector3(
            Mathf.Clamp(Arm3D.Rotation.X, -Mathf.Pi / 2f, Mathf.Pi / 2f),
            Arm3D.Rotation.Y,
            Arm3D.Rotation.Z
        );

        if (TargetNode != null) NodeParentOfCam.GlobalPosition = TargetNode.GlobalPosition;
    }

    public void LookAround(Vector2 newDirection)
    {
        _inputRot = new(-newDirection.Y, -newDirection.X, 0f);
    }
}
