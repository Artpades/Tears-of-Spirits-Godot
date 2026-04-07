#if TOOLS

using Godot;
using System;
namespace GameplayToolkit.ToolsAndEdit
{
	[GlobalClass, Tool, Icon(IconPaths.GIZMOS)]
	public partial class Gizmos3D : Node3D
	{
		[Export] public ExecutionMode executionMode = ExecutionMode.Always;
		public enum ExecutionMode
		{
			Always,
			InEditorOnly,
			InGameOnly
		}
		[Export] public bool filled = false;
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
		}
	}
}
#endif