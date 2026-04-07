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
public partial class TestCollectable2D : Collectable2D
{
	public CharacterBody2D currentBody;
	public override void _Ready()
	{
		base._Ready();
	}

	public override bool CanCollect(Node2D body)
	{
		// if (body is Character2D character)
		// {
		// 	currentBody = character;
		// 	return true;
		// }
		return false;
	}

	public override void ApplyEffect(Node2D body)
	{
		// if (body is Character2D character)
		// {
		// 	currentBody = character;
		// 	PhysicsMovement movement = TypeSearcher.GetNodeByNameOfType<PhysicsMovement>("PhysicsMovement", currentBody);
		// 	movement.maxSpeed += 3;
		// 	movement.JumpVelocity += 3;

		// 	Tween tween = GetTree().CreateTween();
		// 	tween.TweenProperty(this, "scale", new Vector2(0.5f, 0.5f), 0.5f).SetTrans(Tween.TransitionType.Expo).SetEase(Tween.EaseType.Out);
		// 	//QueueFree();
		// }
	}
	public override void OnRespawned()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "scale", Vector2.One, 1f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);

	}
}
