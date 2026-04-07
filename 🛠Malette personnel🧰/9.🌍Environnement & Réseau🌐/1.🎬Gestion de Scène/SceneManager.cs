using Godot;
using Godot.Collections;
using System;

namespace GameplayToolkit.EnvironmentAndNetwork
{
	/// <summary><b>🎬Gestionnaire de Scènes Asynchrone</b><br/><br/>📋 Singleton global responsable du <b>chargement asynchrone</b> des scènes via <see cref="ResourceLoader"/>. Il émet des <b>signaux de progression</b> pour permettre à une UI de réagir.<br/><br/>🎓 Utilisable comme <b>Autoload</b> ou nœud placé en scène. Accès global via <see cref="Instance"/>. Connecter <see cref="LoadingStarted"/>, <see cref="LoadingProgress"/>, <see cref="LoadFinished"/> pour piloter une UI de chargement ou <c>capturer/transférer</c> des données ultérieuement au changement de scène.</summary>
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.SCENEMANAGER)]
#endif
	public partial class SceneManager : Node
	{
		private static SceneManager _instance;
		public static SceneManager Instance => ManagerBuilder.MakeGetter(ref _instance);
		#region  📡 Signaux — Événements de Cycle de Chargement

		/// <summary><b>🚦Chargement Initié📡</b><br/><br/>📋 Émis au <b>démarrage</b> du chargement asynchrone, avant toute progression.<br/><br/>🎓 Connecter ce signal pour <b>afficher un écran de chargement</b> ou désactiver les interactions.</summary>
		[Signal] public delegate void LoadingStartedEventHandler();

		/// <summary><b>📊Progression du Chargement📡</b><br/><br/>📋 Émis à chaque frame pendant le chargement avec le <b>pourcentage d'avancement</b>.<br/><br/>🎓 Connecter pour <b>mettre à jour une barre de progression</b>. Le paramètre <paramref name="percent"/> va de <c>0</c> à <c>100</c>.</summary> <param name="percent">📈 Pourcentage d'avancement du chargement, compris entre <c>0.0</c> et <c>100.0</c>.</param>
		[Signal] public delegate void LoadingProgressEventHandler(float percent);

		/// <summary><b>✅Chargement Terminé📡</b><br/><br/>📋 Émis lorsque la <b>nouvelle scène est active</b>, déclenché par <see cref="SceneTree.SceneChanged"/>.<br/><br/>🎓 Connecter pour <b>masquer l'écran de chargement</b> ou initialiser des éléments post-chargement.</summary>
		[Signal] public delegate void LoadFinishedEventHandler();
		#endregion

		#region 📦 Exportées — Paramètres Configurables depuis l'Éditeur

		/// <summary><b>🗂️Scène Cible du Chargement</b><br/><br/>📋 Référence à la <see cref="PackedScene"/> qui sera chargée de façon asynchrone lors de l'appel à <see cref="LoadSceneAsync"/>.<br/><br/>🎓 Assigner depuis <b>l'inspecteur Godot</b> ou par code avant d'appeler <see cref="LoadSceneAsync"/>.</summary>
		private PackedScene _sceneToLoad;
		#endregion


		#region 🔄 Runtime — État Interne du Gestionnaire

		/// <summary><b>🔒Verrou de Chargement</b><br/><br/>📋 Indique si un <b>chargement est déjà en cours</b>. Empêche les requêtes simultanées dans <see cref="LoadSceneAsync"/>.<br/><br/>🎓 Ne pas modifier manuellement. Géré automatiquement par le cycle de chargement.</summary>
		private bool _isLoading = false;

		/// <summary><b>📊Pourcentage de Progression</b><br/><br/>📋 Stocke la <b>valeur courante de progression</b> du chargement asynchrone, entre <c>0</c> et <c>100</c>.<br/><br/>🎓 Accessible publiquement pour être lu par une <b>UI externe</b> en complément du signal <see cref="LoadingProgress"/>.</summary>
		public float progressPercent;

		#endregion

		//Initialisation du Singleton
		public override void _EnterTree()
		{
			ManagerBuilder.InitInstance(ref _instance, this);
			// 🛑 Ignore l'initialisation si on est dans l'éditeur (mode Tool)
			if (Engine.IsEditorHint()) return;

			// 🔗 À chaque changement de scène effectif, émet LoadFinished pour notifier les abonnés
			GetTree().SceneChanged += () => { EmitSignal(SignalName.LoadFinished); };
		}
		#region 🚀 Méthodes Principales — API Publique du Gestionnaire

		/// <summary><b>🚀Chargement Asynchrone de Scène</b><br/><br/>📋 Démarre le chargement asynchrone de la scène référencée dans <see cref="_sceneToLoad"/>.<br/><br/>🎓 Appeler depuis n'importe quel script pour lancer un changement de scène sans bloquer le thread principal.</summary>
		public void LoadSceneAsync(PackedScene sceneToLoad)
		{
			// Si la scène est nulle ou invalide(ex: chemin incorrect ou scène déjà chargée), on log une erreur et on sort sans faire de requête
			if (sceneToLoad == null)
			{
				if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SCENEMANAGER,
				$"[color=red]⚠️ Aucune scène fournie pour le chargement ![/color]");
				return;
			}

			// 🔒 Vérifie qu'aucun chargement n'est déjà actif pour éviter les requêtes concurrentes
			if (_isLoading)
			{
				if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SCENEMANAGER,
				"[color=yellow]⚠️ Chargement déjà en cours - Requête ignorée[/color]");
				return;
			}

			// 🔒 Pose le verrou de chargement
			_isLoading = true;
			_sceneToLoad = sceneToLoad;

			// 📤 Lance la requête de chargement en arrière-plan
			Error err = ResourceLoader.LoadThreadedRequest(_sceneToLoad.ResourcePath);

			// ❌ Si la requête échoue au démarrage, libère le verrou et sort
			if (err != Error.Ok)
			{
				if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SCENEMANAGER,
				$"[color=red]❌ Erreur démarrage chargement : {err}[/color]");

				_isLoading = false;
				return;
			}
			// 📡 Notifie les abonnés que le chargement vient de commencer
			EmitSignal(SignalName.LoadingStarted);

			if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SCENEMANAGER,
			$"[color=cyan]🚀 Chargement asynchrone démarré : {_sceneToLoad}[/color]");
		}

		#endregion

		#region === Cycle de vie Godot ===

		//Suivi de Progression du Chargement
		public override void _Process(double delta)
		{
			if (_sceneToLoad == null) return;
			// ⏭️ Rien à faire si aucun chargement n'est actif
			if (!_isLoading) return;

			// 📦 Tableau qui recevra la valeur de progression (entre 0.0 et 1.0)
			var progress = new Godot.Collections.Array();

			// Récupère le statut de la requête de chargement
			ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(_sceneToLoad.ResourcePath, progress);

			switch (status)
			{
				// 🔄 Cas : chargement en cours
				case ResourceLoader.ThreadLoadStatus.InProgress:

					// 📊 Convertit la valeur brute (0.0–1.0) en pourcentage (0–100), ou 0 si indisponible
					progressPercent = progress.Count > 0 ? (float)progress[0] * 100f : 0f;

					if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SCENEMANAGER,
					$"[color=white]⏳ Progression : {progressPercent:F1}%[/color]");

					// 📡 Notifie l'UI de la progression actuelle
					EmitSignal(SignalName.LoadingProgress, progressPercent);
					break;

				// ✅ Cas : chargement terminé avec succès
				case ResourceLoader.ThreadLoadStatus.Loaded:

					// 📥 Récupère la ressource chargée depuis le cache de ResourceLoader
					var resource = ResourceLoader.LoadThreadedGet(_sceneToLoad.ResourcePath);

					// 🎬 Si la ressource est bien une PackedScene, effectue le changement de scène en différé
					if (resource is PackedScene packedScene)
					{
						if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SCENEMANAGER,
						$"[color=lime]✅ Chargement terminé ! Changement vers : {_sceneToLoad}[/color]");

						// On change de scène après la frame courante pour éviter les conflits d'état
						GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, packedScene);
					}

					// 🔓 Libère le verrou de chargement
					_isLoading = false;
					break;

				// ❌ Cas : échec du chargement — libère le verrou
				case ResourceLoader.ThreadLoadStatus.Failed:
					// ⚠️ Le chargement a échoué
					if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SCENEMANAGER,
					$"[color=red]❌ Échec du chargement : {_sceneToLoad}[/color]");

					_isLoading = false;
					break;

				// 🛑 Cas : ressource invalide (chemin incorrect) — libère le verrou
				case ResourceLoader.ThreadLoadStatus.InvalidResource:
					// 🛑 Le chemin fourni est invalide
					if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SCENEMANAGER,
					$"[color=red]⚠️ Ressource invalide : {_sceneToLoad}[/color]");

					_isLoading = false;
					break;
			}
		}

		#endregion

	}
}