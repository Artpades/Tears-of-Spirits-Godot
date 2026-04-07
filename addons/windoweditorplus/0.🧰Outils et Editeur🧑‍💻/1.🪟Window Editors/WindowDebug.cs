#if TOOLS
using Godot;

namespace GameplayToolkit.ToolsAndEdit;

[GlobalClass, Tool, Icon(IconPaths.WINDOW)]
public partial class WindowDebug : Window
{
	#region 📡 Signals

	[Signal] public delegate void ParentAnalyzedEventHandler(Node ParentNodeToDebug);

	[Signal] public delegate void WindowOpenedEventHandler();
	#endregion

	#region ⚙️ Configuration

	[ExportCategory("Debug"), Export] private bool OpenOnReady = true;
	#endregion

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (OpenOnReady) OpenWindow();// Vérifie si l’ouverture automatique est activée avant de lancer la fenêtre
	}

	public void OpenWindow()
	{
		// Analyse du parent pour déterminer le type de fenêtre à créer
		HandleNodeDebug(AnalyzeParentNode());

		// Journalisation visuelle confirmant l’ouverture
		CustomLogger.Log(Name, IconPaths.WINDOW, "[wave]Une fenêtre personnalisée a été ouverte.[/wave]");
		// Notification globale via signal
		EmitSignal(nameof(WindowOpened));
	}

	internal Node AnalyzeParentNode()
	{
		// Récupération du nœud parent
		var _nodeToDebug = GetParent();

		CustomLogger.Log(Name, IconPaths.WINDOW,
		"[color=cyan]🔍 Début de l'analyse du parent...[/color]");

		// Émission du signal pour notifier l'analyse
		EmitSignal(nameof(ParentAnalyzed), _nodeToDebug);

		CustomLogger.Log(Name, IconPaths.WINDOW,
		$"[color=lime]✅ Parent analysé : [rainbow]{_nodeToDebug.Name}[/rainbow] - Type: [wave]{_nodeToDebug.GetType().Name}[/wave][/color]");

		return _nodeToDebug;
	}

	internal void HandleNodeDebug(Node _nodeToDebug)
	{
		if (!Engine.IsEditorHint())
		{
			// Ajouter des méthodes dans les fichiers partiels de cette classe selon le type de module

		}
	}

}
#endif