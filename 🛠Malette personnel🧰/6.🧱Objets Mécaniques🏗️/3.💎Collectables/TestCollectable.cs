using GameplayToolkit.CharacterAndAlgorithm;
using GameplayToolkit.MovementAndPhysics;
using Godot;
using System;
namespace GameplayToolkit.ObjectInteractionAndInfluencer;

[GlobalClass,
#if TOOLS
Tool,
#endif
]
public partial class TestCollectable : Collectable3D
{
	public CharacterBody3D currentBody;
	public override void _Ready()
	{
		base._Ready();
	}

	public override bool CanCollect(Node3D body)
	{
		// if (body is Character character)
		// {
		// 	currentBody = character;
		// 	return true;
		// }
		return false;
	}

	public override void ApplyEffect(Node3D body)
	{
		// if (body is Character3D character)
		// {
		// 	currentBody = character;
		// 	PhysicsMovement movement = TypeSearcher.GetNodeByNameOfType<PhysicsMovement>("PhysicsMovement", currentBody);
		// 	movement.maxSpeed += 3;
		// 	movement.JumpVelocity += 3;
			
		// 	Tween tween = GetTree().CreateTween();
		// 	tween.TweenProperty(this, "scale", new Vector3(0.5f, 0.5f, 0.5f), 0.5f).SetTrans(Tween.TransitionType.Expo).SetEase(Tween.EaseType.Out);
		// 	//QueueFree();
		// }
	}
	public override void OnRespawned()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "scale", Vector3.One, 1f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);

	}
}
