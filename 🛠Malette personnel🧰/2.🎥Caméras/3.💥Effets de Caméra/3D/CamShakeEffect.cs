using Godot;
using System;
using System.Diagnostics;
namespace GameplayToolkit.Cameras;

public partial class VirtualCamera3D
{
    [ExportGroup("Camera Shake System")]
#if TOOLS
    [ExportToolButton("Secouer")] public Callable ShakeButton => Callable.From(Shake);
#endif


    #region 📳 Camera Shake System
    [Export] public float SphereStartRadius = 1f;
    [Export] public float Duration = 1f;
    [Export] public Curve SpeedCurve = new(); // Mettre à 0, peut importe où le 0 se situe, çà arrête la courbe
    private Vector3 _originalPosition;
    private Vector3 _shakeOffset = Vector3.Zero;
    private float _limit = 0.001f;
    private bool _shakeActivated;
    private float _currentRadius;
    private float _elapsedTime;
    #endregion

    public void ProcessShakeEffect(float delta)
    {

        if (!_shakeActivated) return;

        // Rétrécir la sphère au fil du temps
        _elapsedTime += delta;
        float progress = Mathf.Clamp(_elapsedTime / Duration, 0f, 1f);

        // Appliquer la courbe de vitesse si définie, sinon atténuation linéaire
        if (SpeedCurve != null)
        {
            float curveValue = SpeedCurve.Sample(progress);
            _currentRadius = SphereStartRadius * curveValue;
        }
        else
        {
            _currentRadius = Mathf.Lerp(SphereStartRadius, 0f, progress);
        }

        // Si la sphère est sous la limite, arrêter le shake
        if (_currentRadius < _limit)
        {
            StopShake();
            return;
        }

        // Téléporter la caméra sur un point aléatoire dans la sphère
        Vector3 randomDirection = new Vector3(
            (float)GD.RandRange(-1.0, 1.0),
            (float)GD.RandRange(-1.0, 1.0),
            (float)GD.RandRange(-1.0, 1.0)
        ).Normalized();

        float randomRadius = (float)GD.RandRange(0, _currentRadius);
        _shakeOffset = randomDirection * randomRadius;

        GlobalPosition = _originalPosition + _shakeOffset;
    }

    public void Shake()
    {
        _originalPosition = GlobalPosition;
        _currentRadius = SphereStartRadius;
        _elapsedTime = 0f;
        _shakeActivated = true;
    }

    private void StopShake()
    {
        _shakeActivated = false;
        GlobalPosition = _originalPosition;
        _shakeOffset = Vector3.Zero;
    }

}
