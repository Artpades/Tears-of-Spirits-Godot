using Godot;
using Godot.Collections;

namespace GameplayToolkit.Cameras;

public partial class VirtualCamera3D
{
    #region 🎯 Look At System
    [ExportGroup("Look At System")]
    [Export] public Array<Node3D> TargetNodes3D = [];

    [ExportSubgroup("Offset")]
    [Export] public Vector3 LookAtOffset3D = Vector3.Zero;

    [ExportSubgroup("Smoothness")]
    [Export] public float LookAtSmoothness = 5.0f;
    [Export] public float ZoomSmoothness = 3.0f;

    [ExportSubgroup("Framing 3D")]
    [Export(PropertyHint.Range, "0.1,0.9")] public float FramingPadding = 0.3f;
    [Export] public float MinDistance = 2.0f;
    [Export] public float MaxDistance = 50.0f;

    [Export] private bool _canLook;
    private bool _is2DMode;
    #endregion

    #region 🎮 3D Processing
    private void ProcessLookAtEffect3D(float delta)
    {
        if (TargetNodes3D.Count == 0) return;

        Vector3 center = CalculateCenter3D();
        float requiredDistance = CalculateRequiredDistance(center);

        Vector3 direction = (center - GlobalPosition).Normalized();
        if (direction.LengthSquared() < 0.001f) return;

        Basis targetBasis = Basis.LookingAt(direction, Vector3.Up);
        Quaternion currentQuat = GlobalBasis.GetRotationQuaternion();
        Quaternion targetQuat = targetBasis.GetRotationQuaternion();
        Quaternion smoothedQuat = currentQuat.Slerp(targetQuat, LookAtSmoothness * delta);
        GlobalBasis = new Basis(smoothedQuat);

        Vector3 camDirection = -GlobalBasis.Z;
        Vector3 targetPosition = center - camDirection * requiredDistance + LookAtOffset3D;
        GlobalPosition = GlobalPosition.Lerp(targetPosition, ZoomSmoothness * delta);

    }

    private Vector3 CalculateCenter3D()
    {
        Vector3 sum = Vector3.Zero;
        foreach (var node in TargetNodes3D)
        {
            sum += node.GlobalPosition;
        }
        return sum / TargetNodes3D.Count;
    }

    private float CalculateRequiredDistance(Vector3 center)
    {
        float maxRadius = 0f;
        foreach (var node in TargetNodes3D)
        {
            float dist = node.GlobalPosition.DistanceTo(center);
            if (dist > maxRadius)
                maxRadius = dist;
        }

        float fovRad = Mathf.DegToRad(Fov);
        float effectiveFov = fovRad * (1.0f - FramingPadding);
        float requiredDistance = maxRadius / Mathf.Tan(effectiveFov / 2.0f);

        return Mathf.Clamp(requiredDistance, MinDistance, MaxDistance);
    }
    #endregion


}
