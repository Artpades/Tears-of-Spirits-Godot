#if TOOLS
using Godot;
using System;
namespace GameplayToolkit.ToolsAndEdit
{
	[GlobalClass, Tool, Icon(IconPaths.WINDOW)]
	public abstract partial class WindowEditor : EditorScript
	{
		[Signal] public delegate void OpenRequestedEventHandler();
		protected Window Viewport;

		public Vector2I Position = new(400, 400);

		public Vector2I Size = new(400, 400);
		public bool AlwaysOnTop;
		// Called when the script is executed (using File -> Run in Script Editor).
		public override void _Run()
		{
			// Gérer une nouvelle instance
			Viewport = new();

			if (AlwaysOnTop) Viewport.AlwaysOnTop = AlwaysOnTop;
			// Créer une fenêtre dans l'éditeur à partir des propriétés renseignées 
			EditorInterface.Singleton.PopupDialog(Viewport, new Rect2I(Position.X, Position.Y, Size.X, Size.Y));

			Viewport.CloseRequested += () =>
			{
				Viewport.QueueFree();
				Viewport = null;
			};

			EmitSignal(SignalName.OpenRequested);
			if (DebugTools.LogWindowEditor) CustomLogger.Log("[[rainbow]WindowEditor[/rainbow]]", IconPaths.WINDOW, "[wave]Fenêtre ouverte dans l'éditeur ! [/wave]");

			InitWindow();
		}

		protected abstract void InitWindow();
	}
}
#endif
