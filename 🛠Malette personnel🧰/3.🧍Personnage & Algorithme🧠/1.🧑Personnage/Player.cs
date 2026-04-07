using Godot;
using System;
namespace GameplayToolkit.CharacterAndAlgorithm
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.PLAYER)]
#endif
	public partial class Player : Character
	{
		[Export] public InputAndControls.PlayerInput PlayerInput { get; set; }
	}
}