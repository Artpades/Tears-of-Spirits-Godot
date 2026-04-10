using GameplayToolkit.InputAndControls;
using GameplayToolkit.MovementAndPhysics;
using GameplayToolkit.UIUX;
using Godot;
using System;
namespace GameplayToolkit.CharacterAndAlgorithm
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.PLAYER)]
#endif
	public partial class Player : Character
	{
		[Export] public PlayerInput PlayerInput { get; set; }
		[Export] public CharacterBodySwimming3D swimming3D;

		private bool _CanInteract;

		private void OnEnteredAreaTriggered(Area3D body)
		{
			var feuFollet = body.GetParent<FeuFollet>();
			if (feuFollet != null)
			{
				_CanInteract = true;

				TypeSearcher.GetFirstNodeOfType<DialogueSystem>().CurrentDialogue = feuFollet.Dialogue;
			}
		}

		private void OnExitedAreaTriggered(Area3D body)
		{
			var feuFollet = body.GetParent<FeuFollet>();

			if (feuFollet != null)
			{
				_CanInteract = false;
			}

		}
		[Export] public bool DialogueMode;

		public void SwitchToDialogueMode()
		{
			if (_CanInteract)
			{
				DialogueMode = true;
				swimming3D._inputDir = Vector2.Zero;
				swimming3D._verticalInput = 0f;
				TypeSearcher.GetFirstNodeOfType<DialogueSystem>().StartDialogue();

				SwitchContext();
			}
		}
		public void SwitchToMoveMode()
		{
			DialogueMode = false;
			TypeSearcher.GetFirstNodeOfType<DialogueSystem>().EndDialogue();

			SwitchContext();

		}
		public void SwitchContext()
		{
			PlayerInput.GetChild<InputContext>(0).ContextActivated = !DialogueMode;
			PlayerInput.GetChild<InputContext>(1).ContextActivated = DialogueMode;
		}
	}
}