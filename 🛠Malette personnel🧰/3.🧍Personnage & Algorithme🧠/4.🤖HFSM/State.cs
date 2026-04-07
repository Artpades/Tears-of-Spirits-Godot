using Godot;
using Godot.Collections;
using System;
namespace GameplayToolkit.CharacterAndAlgorithm
{


#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.STATE)]
#endif
	public partial class State : Resource
	{
		[Signal] public delegate void StartedEventHandler();
		[Signal] public delegate void UpdatedEventHandler();
		[Signal] public delegate void ExitedEventHandler();

		[Export] public Array<State> SubStates;
		public virtual void Start()
		{
			EmitSignal(SignalName.Started);
		}
		public virtual void Update()
		{
			EmitSignal(SignalName.Updated);
		}
		public virtual void Exit()
		{
			EmitSignal(SignalName.Exited);

		}
	}
}
