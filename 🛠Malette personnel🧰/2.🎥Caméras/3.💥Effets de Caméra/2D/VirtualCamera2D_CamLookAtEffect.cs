using Godot;
using Godot.Collections;
using System;

namespace GameplayToolkit.Cameras;

public partial class VirtualCamera2D
{
	#region 🎯 Look At System
	[ExportGroup("LookAtSystem")]
	[Export] public Array<Node2D> LookAtTargets = [];

	[ExportSubgroup("Offset")]
	[Export] public Vector2 LookAtOffset2D = Vector2.Zero;

	[ExportSubgroup("Smoothness")]
	[Export] public float LookAtSmoothness = 5.0f;
	[Export] public float ZoomSmoothness = 3.0f;


	[ExportSubgroup("Framing 2D")]
	[Export] public float MinZoom = 0.5f;
	[Export] public float MaxZoom = 3.0f;
	[Export] public float ScreenMargin = 100.0f;

	[Export] private bool _canLook;
	#endregion



	#region 🎮 2D Processing
	private void ProcessEffect2D(float delta)
	{
		if (LookAtTargets.Count == 0) return;

		Vector2 center = CalculateCenter2D();
		float requiredZoom = CalculateRequiredZoom(center);

		Vector2 targetPosition = center + LookAtOffset2D;
		GlobalPosition = GlobalPosition.Lerp(targetPosition, LookAtSmoothness * delta);

		Vector2 currentZoom = Zoom;
		Vector2 targetZoom = new(requiredZoom, requiredZoom);
		Zoom = currentZoom.Lerp(targetZoom, ZoomSmoothness * delta);


	}

	private Vector2 CalculateCenter2D()
	{
		Vector2 sum = Vector2.Zero;
		foreach (var node in LookAtTargets)
		{
			sum += node.GlobalPosition;
		}
		return sum / LookAtTargets.Count;
	}

	private float CalculateRequiredZoom(Vector2 center)
	{
		float maxDistance = 0f;
		foreach (var node in LookAtTargets)
		{
			float dist = node.GlobalPosition.DistanceTo(center);
			if (dist > maxDistance)
				maxDistance = dist;
		}

		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
		float minViewportDim = Mathf.Min(viewportSize.X, viewportSize.Y) / 2.0f - ScreenMargin;

		float requiredZoom = minViewportDim / Mathf.Max(maxDistance, 1.0f);
		return Mathf.Clamp(requiredZoom, MinZoom, MaxZoom);
	}
	#endregion

	public void StopLookAt()
	{
		_canLook = false;
	}
}
