#if TOOLS

using Godot;
using System;
namespace GameplayToolkit.ToolsAndEdit
{
	[Tool]
	public partial class GizmosCircle2D : Gizmos2D
	{
		[Export] public float Radius = 10;
		public override void _Draw()
		{
			if (Filled) DrawCircle(Vector2.Zero, Radius, Color, Filled, -1, AntiAliasing);
			else
			{
				DrawCircle(Vector2.Zero, Radius, Color, Filled, LineWidth, AntiAliasing);
			}
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			QueueRedraw();
		}
	}
}
#endif
