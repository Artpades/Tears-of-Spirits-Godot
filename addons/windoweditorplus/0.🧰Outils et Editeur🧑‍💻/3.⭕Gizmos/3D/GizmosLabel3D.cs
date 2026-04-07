#if TOOLS
using Godot;
using System;
namespace GameplayToolkit.ToolsAndEdit
{
	[GlobalClass, Tool, Icon(IconPaths.GIZMOS)]
	public partial class GizmosLabel3D : Label3D
	{
		public override void _EnterTree()
		{
			if (!Engine.IsEditorHint())
			{
				QueueFree();
				return;
			}
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
#endif
