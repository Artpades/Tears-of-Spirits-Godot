using GameplayToolkit.InputAndControls;
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
		[Export] public bool DialogueMode;
		public void SwitchContext()
		{
			PlayerInput.GetChild<InputContext>(1).ContextActivated = DialogueMode;
		}
	}
}