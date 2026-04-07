using Godot;
using System;
namespace GameplayToolkit.ObjectInteractionAndInfluencer;
/// <summary><b>⏱️Chronomètre de Nœud</b><br/><br/>📋 Nœud chronomètre capable de <b>démarrer</b>, <b>pauser</b>, <b>stopper</b>, <b>réinitialiser</b> et d'enregistrer des <b>tours (laps)</b>. Émet des signaux à chaque événement clé.<br/><br/>🎓 Utilisable pour mesurer des <b>durées de gameplay</b>, des <b>temps de réaction</b>, ou tout autre besoin de chronométrage. Connecter les signaux pour piloter une <b>UI de chronomètre</b>.</summary>
[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.STOPWATCH)
]
public partial class StopWatch : Node
{
	/// <summary><b>▶️Chronomètre Démarré</b><br/><br/>📋 Émis lorsque le chronomètre <b>démarre</b> via <see cref="Start"/>.<br/><br/>🎓 Connecter pour <b>activer une UI</b> ou déclencher un événement de jeu au lancement du chrono.</summary>
	[Signal] public delegate void StartedEventHandler();

	/// <summary><b>⏹️Chronomètre Arrêté</b><br/><br/>📋 Émis lorsque le chronomètre est <b>stoppé</b> via <see cref="Stop"/>.<br/><br/>🎓 Connecter pour <b>figer une UI</b> ou sauvegarder le temps final.</summary>
	[Signal] public delegate void StoppedEventHandler();

	/// <summary><b>🔁Chronomètre Réinitialisé</b><br/><br/>📋 Émis lorsque le chronomètre est <b>remis à zéro</b> via <see cref="Reset"/>.<br/><br/>🎓 Connecter pour <b>vider l'affichage</b> ou réinitialiser les données de tour côté UI.</summary>
	[Signal] public delegate void WasResetEventHandler();

	/// <summary><b>🏁Tour Enregistré</b><br/><br/>📋 Émis à chaque appel de <see cref="RecordLap"/>, avec le <b>numéro du tour</b> courant.<br/><br/>🎓 Connecter pour <b>afficher la liste des tours</b> ou logger les temps intermédiaires.</summary> <param name="LapEntry">🔢 Numéro du tour enregistré, incrémenté à chaque appel de <see cref="RecordLap"/>.</param>
	[Signal] public delegate void LapRecordedEventHandler(int LapEntry);

	/// <summary><b>📊Temps Mis à Jour</b><br/><br/>📋 Émis à <b>chaque frame</b> pendant que le chronomètre tourne, avec le temps écoulé en secondes.<br/><br/>🎓 Connecter pour mettre à jour un <b>affichage temps réel</b>. Préférer ce signal à un polling manuel de <see cref="ElapsedSeconds"/>.</summary> <param name="timeElapsed">⏱️ Temps écoulé en <b>secondes</b> depuis le dernier <see cref="Start"/> ou <see cref="Reset"/>.</param>
	[Signal] public delegate void TimeUpdatedEventHandler(float timeElapsed);

	/// <summary><b>🖊️Temps Formaté Émis</b><br/><br/>📋 Émis lors de l'appel à <see cref="GetFormattedTime"/>, avec le temps formaté en chaîne <c>HH:MM:SS.mmm</c>.<br/><br/>🎓 Connecter pour recevoir une <b>chaîne prête à afficher</b> sans avoir à appeler <see cref="GetFormattedTime"/> manuellement.</summary> <param name="formattedTime">🖊️ Temps formaté sous la forme <c>HH:MM:SS.mmm</c>, prêt pour un affichage UI.</param>
	[Signal] public delegate void FormattedEventHandler(string formattedTime);

	/// <summary><b>▶️Démarrage Automatique</b><br/><br/>📋 Si <c>true</c>, le chronomètre <b>démarre automatiquement</b> dès que le nœud est prêt, via <see cref="_Ready"/>.<br/><br/>🎓 Activer depuis <b>l'inspecteur</b> pour les cas où aucun déclencheur externe n'est nécessaire.</summary>
	[Export] public bool AutoStart;

	/// <summary><b>🏁Compteur de Tours Interne</b><br/><br/>📋 Stocke le <b>nombre de tours enregistrés</b> depuis le dernier <see cref="Reset"/>.<br/><br/>🎓 Accès en lecture via la propriété publique <see cref="LapsNumber"/>.</summary>
	private int _lapsNumber = 0;

	/// <summary><b>⏱️Temps Écoulé en Secondes</b><br/><br/>📋 Accumule le <b>temps écoulé en secondes</b> à chaque frame pendant que <see cref="_isRunning"/> est <c>true</c>.<br/><br/>🎓 Accès en lecture via la propriété publique <see cref="ElapsedSeconds"/>.</summary>
	private float _elapseSec = 0f;

	/// <summary><b>🔒État de Fonctionnement</b><br/><br/>📋 Indique si le chronomètre est <b>actuellement en train de tourner</b>. Contrôle l'accumulation du temps dans <see cref="_Process"/>.<br/><br/>🎓 Modifié par <see cref="Start"/>, <see cref="Stop"/>, <see cref="SetPause"/> et <see cref="Reset"/>.</summary>
	internal bool _isRunning = false;

	/// <summary><b>⏱️Temps Écoulé (Lecture Seule)</b><br/><br/>📋 Expose en <b>lecture seule</b> le temps écoulé en secondes stocké dans <see cref="_elapseSec"/>.<br/><br/>🎓 Utiliser pour interroger ponctuellement la durée sans s'abonner au signal <see cref="TimeUpdated"/>.</summary>
	public float ElapsedSeconds => _elapseSec;

	/// <summary><b>🏁Nombre de Tours (Lecture Seule)</b><br/><br/>📋 Expose en <b>lecture seule</b> le compteur de tours stocké dans <see cref="_lapsNumber"/>.<br/><br/>🎓 Utiliser pour afficher ou comparer le nombre de tours sans accéder directement au champ privé.</summary>
	public int LapsNumber => _lapsNumber;

	/// <summary><b>🌱 Ready — Initialisation du Nœud</b><br/><br/>📋 Démarre automatiquement le chronomètre si <see cref="AutoStart"/> est <c>true</c> et que le nœud n'est <b>pas en mode éditeur</b>.<br/><br/>🎓 Appelé automatiquement par Godot. Ne pas appeler manuellement.</summary>
	public override void _Ready()
	{
		// ▶️ Démarre le chrono automatiquement si AutoStart est activé et qu'on est en jeu
		if (!Engine.IsEditorHint()) if (AutoStart) Start();
	}

	/// <summary><b>⏱️ Process — Accumulation du Temps</b><br/><br/>📋 Incrémente <see cref="_elapseSec"/> de la durée de chaque frame et émet <see cref="TimeUpdated"/> tant que <see cref="_isRunning"/> est <c>true</c>.<br/><br/>🎓 Appelé automatiquement par Godot à chaque frame. Ne pas appeler manuellement.</summary>
	public override void _Process(double delta)
	{
		if (_isRunning)
		{
			// ⏳ Accumule le temps écoulé en ajoutant la durée de la frame courante
			_elapseSec += (float)delta;
			// 📡 Notifie les abonnés du nouveau temps écoulé
			EmitSignal(SignalName.TimeUpdated, _elapseSec);
		}
	}

	/// <summary><b>▶️Démarrage du Chronomètre</b><br/><br/>📋 Active le chronomètre en passant <see cref="_isRunning"/> à <c>true</c> et émet le signal <see cref="Started"/>.<br/><br/>🎓 Appeler pour <b>lancer ou relancer</b> le chrono. Ne remet pas le temps à zéro — utiliser <see cref="Reset"/> pour cela.</summary>
	public void Start()
	{
		// ✅ Active l'accumulation du temps dans _Process
		_isRunning = true;
		// 📡 Notifie les abonnés que le chrono vient de démarrer
		EmitSignal(SignalName.Started);
	}

	/// <summary><b>⏸️Mise en Pause ou Reprise</b><br/><br/>📋 Suspend ou reprend le chronomètre selon la valeur de <paramref name="isPaused"/>, sans émettre de signal ni modifier le temps écoulé.<br/><br/>🎓 Préférer cette méthode à un arrêt/redémarrage pour les <b>pauses temporaires</b>, afin de ne pas perdre le temps accumulé.</summary><param name="isPaused">⏸️ Si <c>true</c>, suspend le chrono. Si <c>false</c>, le reprend.</param>
	public void SetPause(bool isPaused)
	{
		// ⏸️ Inverse l'état de pause : isPaused=true stoppe l'accumulation, false la reprend
		_isRunning = !isPaused;
	}

	/// <summary><b>⏹️Arrêt du Chronomètre</b><br/><br/>📋 Stoppe le chronomètre en passant <see cref="_isRunning"/> à <c>false</c> et émet le signal <see cref="Stopped"/>. Le temps écoulé est <b>conservé</b>.<br/><br/>🎓 Utiliser pour un <b>arrêt définitif</b>. Pour une pause temporaire, préférer <see cref="SetPause"/>.</summary>
	public void Stop()
	{
		// 🛑 Stoppe l'accumulation du temps
		_isRunning = false;
		// 📡 Notifie les abonnés que le chrono est arrêté
		EmitSignal(SignalName.Stopped);
	}

	/// <summary><b>🔁Réinitialisation Complète</b><br/><br/>📋 Stoppe le chronomètre et remet à zéro le <b>temps écoulé</b> et le <b>compteur de tours</b>, puis émet <see cref="WasReset"/>.<br/><br/>🎓 Appeler avant un nouveau chronomètrage. Pour reprendre sans remettre à zéro, utiliser <see cref="SetPause"/> puis <see cref="Start"/>.</summary>
	public void Reset()
	{
		// 🛑 Stoppe le chrono avant la réinitialisation
		_isRunning = false;
		// 🔄 Remet le temps écoulé à zéro
		_elapseSec = 0f;
		// 🔄 Remet le compteur de tours à zéro
		_lapsNumber = 0;
		// 📡 Notifie les abonnés que le chrono a été réinitialisé
		EmitSignal(SignalName.WasReset);
	}

	/// <summary><b>🏁Enregistrement d'un Tour</b><br/><br/>📋 Incrémente le compteur de tours et émet <see cref="LapRecorded"/> avec le numéro du tour courant. <b>Ignoré</b> si le chrono est arrêté.<br/><br/>🎓 Appeler à chaque passage d'un <b>point de contrôle</b> ou franchissement d'une ligne pour segmenter les temps.</summary>
	public void RecordLap()
	{
		// 🚫 Ignore l'enregistrement si le chrono n'est pas en cours
		if (!_isRunning) return;

		// 🔢 Incrémente le compteur de tours
		_lapsNumber++;
		// 📡 Notifie les abonnés avec le numéro du tour enregistré
		EmitSignal(SignalName.LapRecorded, _lapsNumber);
	}

	/// <summary><b>🖊️ Get Formatted Time — Récupération du Temps Formaté</b><br/><br/>📋 Convertit <see cref="_elapseSec"/> en <see cref="TimeSpan"/>, émet <see cref="Formatted"/> et retourne le temps sous la forme <c>HH:MM:SS.mmm</c>.<br/><br/>🎓 Appeler pour obtenir une <b>chaîne affichable</b> directement dans un <c>Label</c>. Le signal <see cref="Formatted"/> permet aussi de recevoir ce résultat sans stocker le retour.</summary><returns>🖊️ Chaîne formatée <c>HH:MM:SS.mmm</c> représentant le temps écoulé.</returns>
	public string GetFormattedTime()
	{
		// 🕐 Convertit le nombre de secondes brut en structure TimeSpan pour accéder aux composantes
		TimeSpan time = TimeSpan.FromSeconds(_elapseSec);
		// 📡 Émet le temps formaté pour les abonnés au signal Formatted
		EmitSignal(SignalName.Formatted, $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}");
		// ↩️ Retourne également la chaîne formatée pour un usage direct par l'appelant
		return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";
	}
}