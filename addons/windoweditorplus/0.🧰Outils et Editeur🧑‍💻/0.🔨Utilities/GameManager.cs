using Godot;
using System;


namespace GameplayToolkit
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.MANAGERS)]
#endif	
	public partial class GameManager : Node
	{
		public static GameManager Instance { get; private set; }

		public override void _EnterTree()
		{
			Instance = this;
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}
