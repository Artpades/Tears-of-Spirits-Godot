#if TOOLS
using Godot;
using Godot.Collections;
using System;
namespace GameplayToolkit.ToolsAndEdit
{
	[GlobalClass, Tool, Icon(IconPaths.POSTIT)]
	public partial class PostIt : Node
	{
		[Export] public string TaskName;
		[Export(PropertyHint.MultilineText)] public string Description;
		[Export] public bool IsCompleted;
		[Export] public Array<SubTask> subTasks;

		public override void _EnterTree()
		{
			if (!Engine.IsEditorHint())
			{
				QueueFree();

				return;
			}
		}

	}
}
#endif