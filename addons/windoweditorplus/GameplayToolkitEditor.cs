#if TOOLS
using Godot;
using System;
namespace GameplayToolkit.ToolsAndEdit
{
	[Tool]
	public partial class GameplayToolkitEditor : EditorPlugin
	{
		internal Button _WindowOpenerbutton = new();
		private GameplayToolkitCustomWindow gameplayToolkitCustomWindow = new();

		public override void _EnterTree()
		{
			_WindowOpenerbutton.Text = "Ouvrir la fenêtre de menu";
			_WindowOpenerbutton.Pressed += CreateCustomWindow;
			// Initialization of the plugin goes here.
			AddControlToContainer(CustomControlContainer.CanvasEditorMenu, _WindowOpenerbutton);
		}

		public override void _ExitTree()
		{
			_WindowOpenerbutton.Pressed -= CreateCustomWindow;

			// Clean-up of the plugin goes here.
			RemoveControlFromContainer(CustomControlContainer.CanvasEditorMenu, _WindowOpenerbutton);
			_WindowOpenerbutton.QueueFree();
		}

		private void CreateCustomWindow()
		{
			gameplayToolkitCustomWindow._Run();
			GD.Print("Fenêtre ouverte !");
		}
	}
}
#endif
