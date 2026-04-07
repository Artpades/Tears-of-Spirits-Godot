#if TOOLS
using Godot;
using System;
namespace GameplayToolkit
{
	[Tool, Icon(IconPaths.GIZMOS)]
	public partial class Gizmos2D : Node2D
	{
		[Export] public ExecutionMode executionMode = ExecutionMode.Always;
		public enum ExecutionMode
		{
			Always,
			InEditorOnly,
			InGameOnly
		}
		protected bool ShouldExecute()
		{
			return executionMode switch
			{
				ExecutionMode.Always => true,
				ExecutionMode.InEditorOnly => Engine.IsEditorHint(),
				ExecutionMode.InGameOnly => !Engine.IsEditorHint(),
				_ => true
			};
		}
		[Export] public bool Filled = false;
		[Export] public Color Color = new(1, 1, 1, 1);
		[Export] public float LineWidth = 1;
		[Export] public bool AntiAliasing = true;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if(!ShouldExecute())
				return;
		}
		public override void _Draw()
		{
			if(!ShouldExecute())
			{
				// Clear any existing drawings if we shouldn't execute
				QueueRedraw();
				return;
			}
		}
		
	}

}
#endif