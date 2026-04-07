using Godot;
using System;

namespace GameplayToolkit.ObjectInteractionAndInfluencer
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.CLOCK)]
#endif
	public partial class Clock : Node
	{
		// ── Singleton ────────────────────────────────────────────────────────────
		public static Clock Instance { get; private set; }

		// ── Signals (API Godot) ──────────────────────────────────────────────────
		[Signal] public delegate void SecondChangedEventHandler(int second);
		[Signal] public delegate void MinuteChangedEventHandler(int minute);
		[Signal] public delegate void HourChangedEventHandler(int hour);
		[Signal] public delegate void DayChangedEventHandler(int day, int month, int year);

		// ── État interne ─────────────────────────────────────────────────────────
		private int _lastSecond = -1;
		private int _lastMinute = -1;
		private int _lastHour = -1;
		private int _lastDay = -1;

		// ── Propriétés exportées (visibles dans l'éditeur) ───────────────────────
		/// <summary>Format d'affichage de l'heure (tokens : {HH} {MM} {SS}).</summary>
		[Export] public string TimeFormat { get; set; } = "{HH}:{MM}:{SS}";

		/// <summary>Format d'affichage de la date (tokens : {DD} {MM} {YYYY}).</summary>
		[Export] public string DateFormat { get; set; } = "{DD}/{MM}/{YYYY}";

		// ════════════════════════════════════════════════════════════════════════
		//  Cycle de vie Godot
		// ════════════════════════════════════════════════════════════════════════

		public override void _EnterTree()
		{
			Instance = this;

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(Name, IconPaths.CLOCK, "[b]_EnterTree[/b] → Instance [color=cyan]Clock[/color] enregistrée.");
		}

		public override void _Ready()
		{
			if (Engine.IsEditorHint()) return;

			var now = Time.GetDatetimeDictFromSystem();
			_lastSecond = now["second"].AsInt32();
			_lastMinute = now["minute"].AsInt32();
			_lastHour = now["hour"].AsInt32();
			_lastDay = now["day"].AsInt32();

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(Name, IconPaths.CLOCK,
					$"[b]_Ready[/b] → Horloge initialisée — " +
					$"[color=yellow]{_lastHour:D2}:{_lastMinute:D2}:{_lastSecond:D2}[/color] " +
					$"le [color=yellow]{_lastDay:D2}/{now["month"].AsInt32():D2}/{now["year"].AsInt32()}[/color]");
		}

		public override void _Process(double delta)
		{
			if (Engine.IsEditorHint()) return;
			
			var now = Time.GetDatetimeDictFromSystem();

			int second = now["second"].AsInt32();
			int minute = now["minute"].AsInt32();
			int hour = now["hour"].AsInt32();
			int day = now["day"].AsInt32();
			int month = now["month"].AsInt32();
			int year = now["year"].AsInt32();

			if (second != _lastSecond)
			{
				_lastSecond = second;
				EmitSignal(SignalName.SecondChanged, second);

				if (DebugTools.LogModeActivated)
					CustomLogger.Log(Name, IconPaths.CLOCK,
						$"[b]SecondChanged[/b] → Nouvelle seconde : [color=green]{second:D2}[/color]");
			}

			if (minute != _lastMinute)
			{
				_lastMinute = minute;
				EmitSignal(SignalName.MinuteChanged, minute);

				if (DebugTools.LogModeActivated)
					CustomLogger.Log(Name, IconPaths.CLOCK,
						$"[b]MinuteChanged[/b] → Nouvelle minute : [color=green]{minute:D2}[/color]");
			}

			if (hour != _lastHour)
			{
				_lastHour = hour;
				EmitSignal(SignalName.HourChanged, hour);

				if (DebugTools.LogModeActivated)
					CustomLogger.Log(Name, IconPaths.CLOCK,
						$"[b]HourChanged[/b] → Nouvelle heure : [color=green]{hour:D2}h[/color]");
			}

			if (day != _lastDay)
			{
				_lastDay = day;
				EmitSignal(SignalName.DayChanged, day, month, year);

				if (DebugTools.LogModeActivated)
					CustomLogger.Log(Name, IconPaths.CLOCK,
						$"[b]DayChanged[/b] → Nouveau jour : [color=green]{day:D2}/{month:D2}/{year}[/color]");
			}
		}

		// ════════════════════════════════════════════════════════════════════════
		//  API publique – getters atomiques
		// ════════════════════════════════════════════════════════════════════════

		/// <summary>Secondes courantes (0-59).</summary>
		public static int GetCurrentSecond()
		{
			int value = Time.GetDatetimeDictFromSystem()["second"].AsInt32();

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(nameof(Clock), IconPaths.CLOCK,
					$"[b]GetCurrentSecond[/b] → [color=aqua]{value:D2}[/color] secondes");

			return value;
		}

		/// <summary>Minutes courantes (0-59).</summary>
		public static int GetCurrentMinute()
		{
			int value = Time.GetDatetimeDictFromSystem()["minute"].AsInt32();

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(nameof(Clock), IconPaths.CLOCK,
					$"[b]GetCurrentMinute[/b] → [color=aqua]{value:D2}[/color] minutes");

			return value;
		}

		/// <summary>Heure courante (0-23).</summary>
		public static int GetCurrentHour()
		{
			int value = Time.GetDatetimeDictFromSystem()["hour"].AsInt32();

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(nameof(Clock), IconPaths.CLOCK,
					$"[b]GetCurrentHour[/b] → [color=aqua]{value:D2}[/color] heures");

			return value;
		}

		// ════════════════════════════════════════════════════════════════════════
		//  API publique – formatage
		// ════════════════════════════════════════════════════════════════════════

		/// <summary>
		/// Retourne l'heure formatée selon <see cref="TimeFormat"/>.
		/// Tokens disponibles : {HH} {MM} {SS}
		/// </summary>
		public string GetFormattedTime()
		{
			var dict = Time.GetTimeDictFromSystem();
			string result = TimeFormat
				.Replace("{HH}", dict["hour"].AsInt32().ToString("D2"))
				.Replace("{MM}", dict["minute"].AsInt32().ToString("D2"))
				.Replace("{SS}", dict["second"].AsInt32().ToString("D2"));

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(Name, IconPaths.CLOCK,
					$"[b]GetFormattedTime[/b] → [color=yellow]{result}[/color] (format : [i]{TimeFormat}[/i])");

			return result;
		}

		/// <summary>
		/// Retourne la date formatée selon <see cref="DateFormat"/>.
		/// Tokens disponibles : {DD} {MM} {YYYY}
		/// </summary>
		public string GetFormattedDate()
		{
			var dict = Time.GetDateDictFromSystem();
			string result = DateFormat
				.Replace("{DD}", dict["day"].AsInt32().ToString("D2"))
				.Replace("{MM}", dict["month"].AsInt32().ToString("D2"))
				.Replace("{YYYY}", dict["year"].AsInt32().ToString());

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(Name, IconPaths.CLOCK,
					$"[b]GetFormattedDate[/b] → [color=yellow]{result}[/color] (format : [i]{DateFormat}[/i])");

			return result;
		}

		/// <summary>Retourne date + heure au format ISO 8601 via l'API Godot.</summary>
		public static string GetIso8601()
		{
			string result = Time.GetDatetimeStringFromSystem(utc: false, useSpace: false);

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(nameof(Clock), IconPaths.CLOCK,
					$"[b]GetIso8601[/b] → [color=yellow]{result}[/color]");

			return result;
		}

		// ════════════════════════════════════════════════════════════════════════
		//  API publique – timestamp Unix
		// ════════════════════════════════════════════════════════════════════════

		/// <summary>Timestamp Unix en secondes (double précision, via Godot).</summary>
		public static double GetUnixTime()
		{
			double value = Time.GetUnixTimeFromSystem();

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(nameof(Clock), IconPaths.CLOCK,
					$"[b]GetUnixTime[/b] → [color=aqua]{value}[/color] secondes Unix");

			return value;
		}

		/// <summary>Ticks en millisecondes depuis le démarrage du moteur (via Godot).</summary>
		public static ulong GetTicksMsec()
		{
			ulong value = Time.GetTicksMsec();

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(nameof(Clock), IconPaths.CLOCK,
					$"[b]GetTicksMsec[/b] → [color=aqua]{value}[/color] ms depuis le démarrage");

			return value;
		}

		/// <summary>Ticks en microsecondes depuis le démarrage du moteur (via Godot).</summary>
		public static ulong GetTicksUsec()
		{
			ulong value = Time.GetTicksUsec();

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(nameof(Clock), IconPaths.CLOCK,
					$"[b]GetTicksUsec[/b] → [color=aqua]{value}[/color] µs depuis le démarrage");

			return value;
		}
	}
}