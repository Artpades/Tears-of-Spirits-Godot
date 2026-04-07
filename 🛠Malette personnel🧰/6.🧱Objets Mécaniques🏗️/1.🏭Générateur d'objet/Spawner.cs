using Godot;
using Godot.Collections;
using System;

namespace GameplayToolkit.ObjectMechanics
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.SPAWNER)]
#endif
	public partial class Spawner : Node
	{
		#region 🔔 Signaux

		[Signal] public delegate void SpawnedEventHandler();

		#endregion

		#region 📦 Propriétés Exportées

		[Export] public Array<SpawnEntry> Entries;

		[Export] private Node _parentNode;

		#endregion

		#region 🔧 Champs Privés

		private int[] _sequentialSceneIndices;

		private int[] _sequentialPositionIndices;


		private readonly Random _random = new();

		#endregion

		#region 🚀 Cycle de Vie Godot

		public override void _Ready()
		{
			// Résolution du nœud parent où seront ajoutées les instances
			_parentNode ??= GetTree().Root;

			// Initialisation des tableaux d'indices pour les sélections séquentielles
			if (Entries != null)
			{
				_sequentialSceneIndices = new int[Entries.Count];
				_sequentialPositionIndices = new int[Entries.Count];
			}

			// Log de débogage pour confirmer l'initialisation
			if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SPAWNER,
			$"[color=lime]Spawner prêt - Entrées: {Entries?.Count ?? 0}, Parent: {_parentNode?.Name ?? "Root"}[/color]");
		}

		#endregion

		#region 🎯 API Publique

		public void Spawn()
		{
			// Validation de la présence d'entrées
			if (Entries == null || Entries.Count == 0)
			{
				GD.PushWarning("[Spawner]: Aucune entrée définie pour le spawn.");

				if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SPAWNER,
				"[color=red]⚠️ Spawn annulé - Aucune entrée définie[/color]");
				return;
			}

			if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SPAWNER,
			$"[color=yellow]🌱 Début du spawn - {Entries.Count} entrée(s)[/color]");

			// Traitement de chaque configuration de spawn
			foreach (var entry in Entries)
			{
				SpawnEntries(entry);
			}

			// Émission du signal de complétion
			EmitSignal(SignalName.Spawned);

			if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SPAWNER,
			"[color=green]✓ Spawn terminé avec succès[/color]");
		}

		#endregion

		#region 🧩 Logique de Génération

		private void SpawnEntries(SpawnEntry entry)
		{
			// Validation de l'entrée
			if (entry == null)
			{
				GD.PushWarning("Spawner: Entrée nulle détectée.");
				return;
			}

			// Validation de la présence de scènes
			if (entry.Scenes == null || entry.Scenes.Count == 0)
			{
				GD.PushWarning("Spawner: Aucune scène définie dans l'entrée.");
				return;
			}

			// Si aucune position n'est définie, utilise les enfants comme positions de spawn
			if (entry.Positions == null || entry.Positions.Count == 0)
			{
				entry.Positions = [];

				// Parcours des enfants du spawner pour trouver les nœuds de position
				foreach (Node child in GetChildren())
				{
					// Seuls les nœuds 2D ou 3D sont considérés comme positions valides
					if (child is Node3D || child is Node2D)
					{
						entry.Positions.Add(GetPathTo(child));
					}
				}

				// Vérification qu'au moins une position a été trouvée
				if (entry.Positions.Count == 0)
				{
					GD.PushWarning("Spawner: Aucune position définie dans l'entrée et aucun nœud enfant (Node2D/Node3D) disponible.");
					return;
				}
			}

			// Récupération de l'index de l'entrée pour les sélections séquentielles
			int entryIndex = Entries.IndexOf(entry);

			// Sélection des scènes selon le mode défini
			var scenesToSpawn = SelectScenes(entry, entryIndex);

			// Sélection des positions selon le mode défini
			var positionsToUse = SelectPositions(entry, entryIndex);

			// Instanciation et positionnement de chaque scène
			for (int i = 0; i < scenesToSpawn.Length; i++)
			{
				var scene = scenesToSpawn[i];
				// Utilisation du modulo pour réutiliser les positions si moins nombreuses que les scènes
				var positionPath = positionsToUse[i % positionsToUse.Length];

				// Validation de la scène
				if (scene == null)
				{
					GD.PushWarning("Spawner: Scène nulle détectée.");

					if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SPAWNER,
					"[color=red]⚠️ Scène nulle détectée, spawn ignoré[/color]");
					continue;
				}

				// Instanciation de la scène
				var instance = scene.Instantiate();

				// Ajout de l'instance au nœud parent
				_parentNode.AddChild(instance);

				// Résolution du nœud de position
				Node positionNode = GetNode(positionPath);

				// Application de la transformation 3D si les types correspondent
				if (positionNode is Node3D node3D && instance is Node3D instance3D)
				{
					instance3D.GlobalPosition = node3D.GlobalPosition;
					instance3D.GlobalRotation = node3D.GlobalRotation;

					if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SPAWNER,
					$"[color=cyan]📍 Spawn 3D: {instance.Name} à Position: {node3D.GlobalPosition}[/color]");
				}
				// Application de la transformation 2D si les types correspondent
				else if (positionNode is Node2D node2D && instance is Node2D instance2D)
				{
					instance2D.GlobalPosition = node2D.GlobalPosition;
					instance2D.GlobalRotation = node2D.GlobalRotation;

					if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SPAWNER,
					$"[color=cyan]📍 Spawn 2D: {instance.Name} à Position: {node2D.GlobalPosition}[/color]");
				}
			}
		}

		#endregion

		#region 🎛️ Assistants de Sélection

		private PackedScene[] SelectScenes(SpawnEntry entry, int entryIndex)
		{
			return entry.ScenesSelectionMode switch
			{
				// Retourne toutes les scènes disponibles
				SpawnEntry.SelectionMode.All => [.. entry.Scenes],
				// Retourne une scène choisie aléatoirement
				SpawnEntry.SelectionMode.Random => [entry.Scenes[_random.Next(entry.Scenes.Count)]],
				// Retourne la prochaine scène dans la séquence
				SpawnEntry.SelectionMode.Sequential => [GetSequentialScene(entry, entryIndex)],
				// Cas par défaut: retourne la première scène
				_ => [entry.Scenes[0]]
			};
		}

		private NodePath[] SelectPositions(SpawnEntry entry, int entryIndex)
		{
			return entry.PosSelectionMode switch
			{
				// Retourne toutes les positions disponibles
				SpawnEntry.SelectionMode.All => [.. entry.Positions],
				// Retourne une position choisie aléatoirement
				SpawnEntry.SelectionMode.Random => [entry.Positions[_random.Next(entry.Positions.Count)]],
				// Retourne la prochaine position dans la séquence
				SpawnEntry.SelectionMode.Sequential => [GetSequentialPosition(entry, entryIndex)],
				// Cas par défaut: retourne la première position
				_ => [entry.Positions[0]]
			};
		}

		private PackedScene GetSequentialScene(SpawnEntry entry, int entryIndex)
		{
			// Récupération de la scène à l'index actuel
			var scene = entry.Scenes[_sequentialSceneIndices[entryIndex]];
			// Incrémentation de l'index avec retour au début via modulo
			_sequentialSceneIndices[entryIndex] = (_sequentialSceneIndices[entryIndex] + 1) % entry.Scenes.Count;
			return scene;
		}

		private NodePath GetSequentialPosition(SpawnEntry entry, int entryIndex)
		{
			// Récupération de la position à l'index actuel
			var position = entry.Positions[_sequentialPositionIndices[entryIndex]];
			// Incrémentation de l'index avec retour au début via modulo
			_sequentialPositionIndices[entryIndex] = (_sequentialPositionIndices[entryIndex] + 1) % entry.Positions.Count;
			return position;
		}

		#endregion
	}
}