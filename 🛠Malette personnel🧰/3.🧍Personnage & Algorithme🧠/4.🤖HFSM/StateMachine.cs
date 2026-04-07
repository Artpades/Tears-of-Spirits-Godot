using Godot;
using Godot.Collections;
using System;
namespace GameplayToolkit.CharacterAndAlgorithm
{


#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.HFSM)]
#endif
	public partial class StateMachine : Node
	{
		[Signal] public delegate void SuperStateSwitchedEventHandler(State newSuperState);
		[Export] public Array<State> SuperStates;
		[Export] public State InitialState;
		[Export] public State CurrentState;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			if (Engine.IsEditorHint()) return;
			if (InitialState == null) CurrentState = SuperStates[0];

			CurrentState.Start();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (Engine.IsEditorHint()) return;

			CurrentState.Update();
		}

		public void Switch(State newSuperState)
		{
			if (newSuperState == null) return;
			CurrentState.Exit();
			CurrentState = newSuperState;
			CurrentState.Start();
			EmitSignal(SignalName.SuperStateSwitched, newSuperState);
		}
	}
}
