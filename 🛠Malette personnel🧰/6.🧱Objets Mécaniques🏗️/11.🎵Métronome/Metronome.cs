using Godot;
using System;

namespace GameplayToolkit.ObjectInteractionAndInfluencer
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.METRONOME)]
#endif
	public partial class Metronome : Node
	{
		// ── Signals ──────────────────────────────────────────────────────────
		/// <summary>Emitted on every tick (beat).</summary>
		[Signal] public delegate void TickEventHandler(int beatIndex);

		/// <summary>Emitted on the first beat of each measure.</summary>
		[Signal] public delegate void MeasureEventHandler(int measureIndex);

		/// <summary>Emitted on a specific subdivision (e.g. eighth-note, sixteenth-note).</summary>
		[Signal] public delegate void SubdivisionEventHandler(int subdivisionIndex);

		/// <summary>Emitted when the metronome starts or resumes.</summary>
		[Signal] public delegate void StartedEventHandler();

		/// <summary>Emitted when the metronome is paused.</summary>
		[Signal] public delegate void PausedEventHandler();

		/// <summary>Emitted when the metronome is stopped and reset.</summary>
		[Signal] public delegate void StoppedEventHandler();

		// ── Exported properties ───────────────────────────────────────────────
		/// <summary>Bâttements par minute.</summary>
		[Export(PropertyHint.Range, "1,960,1")]
		public float Bpm
		{
			get => _bpm;
			set
			{
				_bpm = Mathf.Clamp(value, 1f, 960f);
				_beatInterval = 60.0 / _bpm;
			}
		}

		/// <summary>Number of beats per measure (numerator of time signature).</summary>
		[Export(PropertyHint.Range, "1,32,1")]
		public int BeatsPerMeasure { get; set; } = 4;

		/// <summary>Number of subdivisions per beat (1 = no subdivision, 2 = eighth-notes, 4 = sixteenth-notes…).</summary>
		[Export(PropertyHint.Range, "1,16,1")]
		public int SubdivisionsPerBeat { get; set; } = 1;

		/// <summary>Start playing automatically when the node is ready.</summary>
		[Export] public bool AutoStart { get; set; } = false;

		// ── State ─────────────────────────────────────────────────────────────
		public enum MetronomeState { Stopped, Playing, Paused }

		public MetronomeState State { get; private set; } = MetronomeState.Stopped;

		/// <summary>Current beat index within the current measure (0-based).</summary>
		public int CurrentBeat { get; private set; } = 0;

		/// <summary>Total beats elapsed since Start().</summary>
		public int TotalBeats { get; private set; } = 0;

		/// <summary>Total measures elapsed since Start().</summary>
		public int TotalMeasures { get; private set; } = 0;

		/// <summary>Current subdivision index within the current beat (0-based).</summary>
		public int CurrentSubdivision { get; private set; } = 0;

		/// <summary>Total subdivisions elapsed since Start().</summary>
		public int TotalSubdivisions { get; private set; } = 0;

		// ── Private fields ────────────────────────────────────────────────────
		private float _bpm = 120f;
		private double _beatInterval = 0.5;
		private double _subdivisionInterval = 0.5;
		private double _accumulator = 0.0;
		private double _subdivisionAccumulator = 0.0;

		// ── Lifecycle ─────────────────────────────────────────────────────────
		public override void _Ready()
		{
			if (Engine.IsEditorHint()) return;
			Bpm = _bpm;

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(Name, IconPaths.METRONOME,
					$"[b]_Ready[/b] — Métronome initialisé. [color=cyan]BPM[/color] : [b]{_bpm}[/b] | [color=cyan]Mesure[/color] : [b]{BeatsPerMeasure}/4[/b] | [color=cyan]Subdivisions[/color] : [b]{SubdivisionsPerBeat}[/b] | [color=cyan]AutoStart[/color] : [b]{AutoStart}[/b]");

			if (AutoStart)
				Start();
		}

		public override void _Process(double delta)
		{
			if (State != MetronomeState.Playing)
				return;

			if (SubdivisionsPerBeat > 1)
			{
				_subdivisionAccumulator += delta;
				while (_subdivisionAccumulator >= _subdivisionInterval)
				{
					_subdivisionAccumulator -= _subdivisionInterval;
					EmitSignal(SignalName.Subdivision, TotalSubdivisions);

					if (DebugTools.LogModeActivated)
						CustomLogger.Log(Name, IconPaths.METRONOME,
							$"[b]Subdivision[/b] — [color=yellow]♩.[/color] Subdivision [b]#{TotalSubdivisions}[/b] | Beat courant : [b]{CurrentBeat + 1}/{BeatsPerMeasure}[/b] | Sous-division : [b]{CurrentSubdivision + 1}/{SubdivisionsPerBeat}[/b]");

					TotalSubdivisions++;
					CurrentSubdivision = (CurrentSubdivision + 1) % SubdivisionsPerBeat;
				}
			}

			_accumulator += delta;
			while (_accumulator >= _beatInterval)
			{
				_accumulator -= _beatInterval;
				FireBeat();
			}
		}

		// ── Public API ────────────────────────────────────────────────────────

		/// <summary>Start the metronome from zero (or resume if paused).</summary>
		public void Start()
		{
			if (State == MetronomeState.Playing)
				return;

			if (State == MetronomeState.Stopped)
				ResetCounters();

			State = MetronomeState.Playing;
			EmitSignal(SignalName.Started);

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(Name, IconPaths.METRONOME,
					$"[b]Start[/b] — [color=green]▶ Métronome démarré.[/color] [color=cyan]BPM[/color] : [b]{_bpm}[/b] | [color=cyan]Intervalle[/color] : [b]{_beatInterval:F4}s[/b]");
		}

		/// <summary>Pause the metronome without resetting position.</summary>
		public void Pause()
		{
			if (State != MetronomeState.Playing)
				return;

			State = MetronomeState.Paused;
			EmitSignal(SignalName.Paused);

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(Name, IconPaths.METRONOME,
					$"[b]Pause[/b] — [color=orange]⏸ Métronome mis en pause.[/color] Beat : [b]{CurrentBeat + 1}/{BeatsPerMeasure}[/b] | Total beats : [b]{TotalBeats}[/b] | Total mesures : [b]{TotalMeasures}[/b]");
		}

		/// <summary>Resume after a pause (alias for Start).</summary>
		public void Resume() => Start();

		/// <summary>Stop the metronome and reset all counters.</summary>
		public void Stop()
		{
			if (State == MetronomeState.Stopped)
				return;

			State = MetronomeState.Stopped;
			ResetCounters();
			EmitSignal(SignalName.Stopped);

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(Name, IconPaths.METRONOME,
					$"[b]Stop[/b] — [color=red]⏹ Métronome arrêté et réinitialisé.[/color] Beats joués : [b]{TotalBeats}[/b] | Mesures jouées : [b]{TotalMeasures}[/b]");
		}

		/// <summary>Toggle between playing and paused states.</summary>
		public void TogglePlayPause()
		{
			if (DebugTools.LogModeActivated)
				CustomLogger.Log(Name, IconPaths.METRONOME,
					$"[b]TogglePlayPause[/b] — État actuel : [color=cyan][b]{State}[/b][/color] → basculement en cours…");

			if (State == MetronomeState.Playing)
				Pause();
			else
				Start();
		}

		/// <summary>Change the BPM at runtime without stopping the metronome.</summary>
		public void SetBpmSmooth(float newBpm)
		{
			float oldBpm = _bpm;

			if (_beatInterval > 0)
			{
				double progress = _accumulator / _beatInterval;
				Bpm = newBpm;
				_accumulator = progress * _beatInterval;

				if (SubdivisionsPerBeat > 1)
				{
					double subProgress = _subdivisionAccumulator / _subdivisionInterval;
					_subdivisionInterval = _beatInterval / SubdivisionsPerBeat;
					_subdivisionAccumulator = subProgress * _subdivisionInterval;
				}
			}
			else
			{
				Bpm = newBpm;
			}

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(Name, IconPaths.METRONOME,
					$"[b]SetBpmSmooth[/b] — [color=cyan]BPM[/color] : [color=red][b]{oldBpm}[/b][/color] → [color=green][b]{_bpm}[/b][/color] | Nouvel intervalle : [b]{_beatInterval:F4}s[/b] | Phase conservée.");
		}

		// ── Private helpers ───────────────────────────────────────────────────
		private void FireBeat()
		{
			EmitSignal(SignalName.Tick, TotalBeats);

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(Name, IconPaths.METRONOME,
					$"[b]Tick[/b] — [color=yellow]♩[/color] Beat [b]#{TotalBeats}[/b] | Beat dans la mesure : [b]{CurrentBeat + 1}/{BeatsPerMeasure}[/b]");

			TotalBeats++;

			if (CurrentBeat == 0)
			{
				EmitSignal(SignalName.Measure, TotalMeasures);

				if (DebugTools.LogModeActivated)
					CustomLogger.Log(Name, IconPaths.METRONOME,
						$"[b]Measure[/b] — [color=lime]𝄞 Nouvelle mesure [b]#{TotalMeasures}[/b][/color] | Total beats : [b]{TotalBeats}[/b]");

				TotalMeasures++;
			}

			CurrentBeat = (CurrentBeat + 1) % BeatsPerMeasure;

			if (SubdivisionsPerBeat > 1)
			{
				_subdivisionAccumulator = 0.0;
				_subdivisionInterval = _beatInterval / SubdivisionsPerBeat;
				CurrentSubdivision = 0;
			}
		}

		private void ResetCounters()
		{
			_accumulator = 0.0;
			_subdivisionAccumulator = 0.0;
			CurrentBeat = 0;
			TotalBeats = 0;
			TotalMeasures = 0;
			CurrentSubdivision = 0;
			TotalSubdivisions = 0;
			_subdivisionInterval = _beatInterval / Mathf.Max(1, SubdivisionsPerBeat);

			if (DebugTools.LogModeActivated)
				CustomLogger.Log(Name, IconPaths.METRONOME,
					$"[b]ResetCounters[/b] — [color=orange]🔄 Tous les compteurs ont été remis à zéro.[/color] Intervalle beat : [b]{_beatInterval:F4}s[/b] | Intervalle subdivision : [b]{_subdivisionInterval:F4}s[/b]");
		}
	}
}