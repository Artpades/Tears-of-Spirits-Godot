#if TOOLS

using Godot;
using System;
namespace GameplayToolkit.ToolsAndEdit
{
	[Tool]
	public partial class GizmosLine2D : Gizmos2D
	{
		[Export] public Vector2 EndPoint = Vector2.Right * 100;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			QueueRedraw();
		}
		public override void _Draw()
		{
			base._Draw();
			DrawLine(Vector2.Zero, EndPoint, Color, LineWidth, AntiAliasing);
		}
	}
}
#endif