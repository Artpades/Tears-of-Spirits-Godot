using Godot;
using Godot.Collections;
using System;
namespace GameplayToolkit.ObjectInteractionAndInfluencer;

[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.COLLECTABLE)
]
public abstract partial class Collectable3D : Area3D
{
	[Signal] public delegate void CollectedEventHandler(Node3D body);
	[Signal] public delegate void RespawnedEventHandler();

	[Export] public float AttractionRadius = 1f;
	[Export] public float AttractionSpeed = 5f;
	[Export] public float RespawnDelay = 5f; // 0 = pas de respawn

	private bool _collected = false;
	private Timer _respawnTimer;

	public bool IsCollected => _collected;

	public override void _Ready()
	{
		// Création du timer de respawn
		_respawnTimer = new()
		{
			OneShot = true
		};
		_respawnTimer.Timeout += Respawn;
		AddChild(_respawnTimer);

		BodyEntered += Collect;
	}

	public override void _Process(double delta)
	{
		if (_collected) return;

		foreach (var body in GetOverlappingBodies())
		{
			if (!CanCollect(body)) continue;

			float distance = GlobalPosition.DistanceTo(body.GlobalPosition);

			if (distance <= AttractionRadius)
			{
				GlobalPosition = GlobalPosition.Lerp(
					body.GlobalPosition,
					AttractionSpeed * (float)delta
				);
				return; // On cible le premier body valide uniquement
			}
		}
	}

	public void Collect(Node3D body)
	{
		if (_collected) return;
		if (!CanCollect(body)) return;

		_collected = true;
		ApplyEffect(body);
		EmitSignal(SignalName.Collected, body);


		// Désactive les collisions pour éviter de re-collecter
		CallDeferred(MethodName.SetActiveColliders, false);
		SetDeferred(Area3D.PropertyName.Monitoring, false);
		// Lance le respawn si configuré
		if (RespawnDelay > 0f)
			_respawnTimer.Start(RespawnDelay);
	}

	public void Respawn()
	{
		_collected = false;

		OnRespawned();

		CallDeferred(MethodName.SetActiveColliders, true);
		SetDeferred(Area3D.PropertyName.Monitoring, true);
		
		EmitSignal(SignalName.Respawned);
	}

	private void SetActiveColliders(bool setActive)
	{
		Array<CollisionShape3D> colliders = TypeSearcher.GetChildrenOfType<CollisionShape3D>(this);
		for (int i = 0; i < colliders.Count; i++)
		{
			colliders[i].Disabled = !setActive;
		}
	}
	public abstract bool CanCollect(Node3D body);
	public abstract void ApplyEffect(Node3D body);
	public abstract void OnRespawned();
}