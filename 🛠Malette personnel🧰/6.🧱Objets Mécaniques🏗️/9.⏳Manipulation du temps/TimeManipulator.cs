using Godot;
using System;

namespace GameplayToolkit.ObjectInteractionAndInfluencer
{

	[GlobalClass,
#if TOOLS
	Tool,
#endif
	 Icon(IconPaths.TIMEMANIPULATOR)]
	public partial class TimeManipulator : Node
	{
		#region Exports

		[Export] public NodePath TargetNodePath { get; set; }

		[Export] public float TimeScale { get; set; } = 1.0f;

		[Export] public bool ApplyOnReady { get; set; } = true;

		[Export] public bool AffectAnimationPlayers { get; set; } = true;

		#endregion


		#region Private Fields

		private Node _targetNode;
		private Godot.Collections.Array<AnimationPlayer> _animationPlayers = [];

		#endregion

		#region Godot Lifecycle

		public override void _Ready()
		{
			if (TargetNodePath != null && !TargetNodePath.IsEmpty)
			{
				_targetNode = GetNode<Node>(TargetNodePath);

				if (_targetNode != null)
				{
					// Collecter tous les AnimationPlayer si nécessaire
					if (AffectAnimationPlayers)
					{
						CollectAnimationPlayers(_targetNode);

						if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.TIMEMANIPULATOR,
						$"[color=cyan]🎬 {_animationPlayers.Count} AnimationPlayer(s) collecté(s)[/color]");
					}

					// Appliquer le TimeScale initial
					if (ApplyOnReady)
					{
						ApplyTimeScale(TimeScale);

						if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.TIMEMANIPULATOR,
						$"[color=lime]✅ TimeManipulator initialisé - Cible: {_targetNode.Name}, TimeScale: {TimeScale}[/color]");
					}
				}
				else
				{
					GD.PushWarning($"TimeManipulator: Le nœud cible '{TargetNodePath}' n'a pas été trouvé.");

					if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.TIMEMANIPULATOR,
					$"[color=red]❌ Nœud cible introuvable : '{TargetNodePath}'[/color]");
				}
			}
			else
			{
				GD.PushWarning("TimeManipulator: Aucun nœud cible n'a été défini.");

				if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.TIMEMANIPULATOR,
				"[color=yellow]⚠️ Aucun nœud cible défini[/color]");
			}

		}

		public override void _Process(double delta)
		{
			// Mise à jour en temps réel si le TimeScale change dans l'éditeur
			if (Engine.IsEditorHint())
			{
				return;
			}
		}


		#endregion

		#region Public Methods

		public void ApplyTimeScale(float scale)
		{
			if (_targetNode == null)
			{
				GD.PushWarning("TimeManipulator: Aucun nœud cible défini.");

				if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.TIMEMANIPULATOR,
				"[color=red]❌ Aucun nœud cible défini[/color]");
				return;
			}

			TimeScale = Mathf.Max(0.0f, scale);

			// Si TimeScale est 0, mettre en pause
			if (TimeScale == 0.0f)
			{
				_targetNode.ProcessMode = ProcessModeEnum.Disabled;

				// Mettre en pause les AnimationPlayers
				foreach (var animPlayer in _animationPlayers)
				{
					if (animPlayer != null && animPlayer.IsPlaying())
					{
						animPlayer.Pause();
					}
				}

				if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.TIMEMANIPULATOR,
				$"[color=yellow]⏸️ Nœud '{_targetNode.Name}' mis en PAUSE[/color]");
			}
			else
			{
				// Sinon, s'assurer que le nœud est actif
				if (_targetNode.ProcessMode != ProcessModeEnum.Inherit)
				{
					_targetNode.ProcessMode = ProcessModeEnum.Inherit;
				}

				// Appliquer le TimeScale aux AnimationPlayers
				foreach (var animPlayer in _animationPlayers)
				{
					if (animPlayer != null)
					{
						animPlayer.SpeedScale = TimeScale;
						animPlayer.Play();
					}
				}

				if (DebugTools.LogModeActivated)
				{
					string speedIcon = TimeScale < 1.0f ? "🐌" : (TimeScale > 1.0f ? "⚡" : "▶️");
					string speedText = TimeScale < 1.0f ? "RALENTI" : (TimeScale > 1.0f ? "ACCÉLÉRÉ" : "NORMAL");
					string color = TimeScale < 1.0f ? "cyan" : (TimeScale > 1.0f ? "magenta" : "white");

					CustomLogger.Log(Name, IconPaths.TIMEMANIPULATOR,
					$"[color={color}]{speedIcon} TimeScale appliqué : {TimeScale:F2}x ({speedText}) - Cible: {_targetNode.Name}[/color]");
				}
			}
		}

		public void Pause()
		{
			ApplyTimeScale(0.0f);
		}

		public void Resume()
		{
			ApplyTimeScale(1.0f);
		}

		public void ResetTimeScale()
		{
			ApplyTimeScale(1.0f);
		}

		public void SetTargetNode(Node newTarget)
		{
			if (newTarget == null)
			{
				GD.PushWarning("TimeManipulator: Le nouveau nœud cible est null.");

				if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.TIMEMANIPULATOR,
				"[color=red]❌ Nouveau nœud cible est null[/color]");
				return;
			}

			_targetNode = newTarget;
			TargetNodePath = GetPathTo(newTarget);

			_animationPlayers.Clear();
			if (AffectAnimationPlayers)
			{
				CollectAnimationPlayers(_targetNode);
			}

			ApplyTimeScale(TimeScale);

			if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.TIMEMANIPULATOR,
			$"[color=lime]🎯 Nouveau nœud cible défini : {newTarget.Name}[/color]");
		}
		public Node GetTargetNode()
		{
			return _targetNode;
		}

		public bool IsPaused()
		{
			return TimeScale == 0.0f;
		}


		#endregion

		#region Private Methods

		private void CollectAnimationPlayers(Node node)
		{
			if (node == null) return;

			// Vérifier si le nœud actuel est un AnimationPlayer
			if (node is AnimationPlayer animPlayer)
			{
				_animationPlayers.Add(animPlayer);
			}

			// Parcourir récursivement les enfants
			foreach (Node child in node.GetChildren())
			{
				CollectAnimationPlayers(child);
			}
		}


		#endregion
	}
}