#if TOOLS

using Godot;
using System;
namespace GameplayToolkit.ToolsAndEdit
{
	[GlobalClass, Tool]
	public partial class GizmosRectangle2D : Gizmos2D
	{
		[Export] public Vector2 RectPoint = Vector2.Zero;
		[Export] public Vector2 Size = Vector2.One * 100;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}
		public override void _Draw()
		{
			if (Filled) DrawRect(new Rect2(Vector2.Zero, RectPoint), Color, Filled, -1, AntiAliasing);
			else
			{
				DrawRect(new Rect2(Vector2.Zero, RectPoint), Color, Filled, LineWidth, AntiAliasing);
			}
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			QueueRedraw();
		}
	}
}
#endif