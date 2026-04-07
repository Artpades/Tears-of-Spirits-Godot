using Godot;
using System;

namespace GameplayToolkit.VisualAndSoundEffects.SFX;

[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.MUSICMANAGER)
]
public partial class MusicManager : Node
{
	[Export] public AudioStreamMP3 AudioForStart;
	[Export] public AudioStreamMP3 AudioForLoop;

	[ExportGroup("Loop Trigger")]
	/// <summary>
	/// Secondes avant la fin de l'intro où la boucle démarre en avance.
	/// Mettre 0 pour démarrer exactement à la fin.
	/// </summary>
	[Export(PropertyHint.Range, "0,30,0.01")]
	public float OverlapSeconds = 0f;

	/// <summary>
	/// Si > 0, on fait un crossfade entre l'intro et la boucle sur cette durée (en secondes).
	/// Doit être <= OverlapSeconds pour avoir un effet.
	/// </summary>
	[Export(PropertyHint.Range, "0,10,0.01")]
	public float CrossfadeDuration = 0f;

	public static MusicManager Instance { get; private set; }


	private AudioStreamPlayer _introPlayer;
	private AudioStreamPlayer _loopPlayer;

	private bool _isPlaying = false;
	private bool _loopTriggered = false; // a-t-on déjà déclenché la boucle pendant l'intro ?
	private bool _crossfading = false;
	private double _crossfadeTimer = 0.0;


	public override void _Ready()
	{
		Instance = this;
		if (Engine.IsEditorHint()) return;

		_introPlayer = new AudioStreamPlayer();
		_loopPlayer = new AudioStreamPlayer();

		CallDeferred(MethodName.AddAudioStreamToTree);

		_introPlayer.Finished += OnIntroFinished;

		if (AudioForLoop is not null)
			AudioForLoop.Loop = true;
	}
	internal void AddAudioStreamToTree()
	{
		GetTree().Root.AddChild(_introPlayer);
		GetTree().Root.AddChild(_loopPlayer);
	}
	public override void _Process(double delta)
	{

		if (!_isPlaying || _loopTriggered) return;
		if (AudioForStart is null || !_introPlayer.Playing) return;

		double totalLength = AudioForStart.GetLength();
		double currentPos = _introPlayer.GetPlaybackPosition();
		double timeLeft = totalLength - currentPos;

		// On déclenche la boucle quand il reste OverlapSeconds secondes
		if (timeLeft <= OverlapSeconds)
		{
			_loopTriggered = true;
			TriggerLoopEarly();
		}

		// Gestion du crossfade en cours
		if (_crossfading)
		{
			_crossfadeTimer += delta;
			float t = Mathf.Clamp((float)(_crossfadeTimer / CrossfadeDuration), 0f, 1f);

			_introPlayer.VolumeDb = Mathf.LinearToDb(1f - t);
			_loopPlayer.VolumeDb = Mathf.LinearToDb(t);

			if (t >= 1f)
			{
				_crossfading = false;
				_introPlayer.Stop();
				_introPlayer.VolumeDb = 0f; // reset pour la prochaine fois
			}
		}
	}

	// ─── API publique ──────────────────────────────────────────

	public void Play()
	{
		if (_isPlaying) return;
		_isPlaying = true;
		_loopTriggered = false;

		if (AudioForStart is not null)
		{
			_introPlayer.VolumeDb = 0f;
			_loopPlayer.VolumeDb = CrossfadeDuration > 0f ? Mathf.LinearToDb(0f) : 0f;
			_introPlayer.Stream = AudioForStart;
			_introPlayer.Play();
		}
		else
		{
			StartLoop();
		}
	}

	public void Stop()
	{
		_introPlayer.Stop();
		_loopPlayer.Stop();
		_isPlaying = false;
		_loopTriggered = false;
		_crossfading = false;
	}

	public void SetPaused(bool paused)
	{
		_introPlayer.StreamPaused = paused;
		_loopPlayer.StreamPaused = paused;
	}

	// ─── Privé ────────────────────────────────────────────────

	private void TriggerLoopEarly()
	{
		_loopPlayer.Stream = AudioForLoop;

		if (CrossfadeDuration > 0f)
		{
			// On part d'un volume nul et on monte progressivement dans _Process
			_loopPlayer.VolumeDb = Mathf.LinearToDb(0f);
			_loopPlayer.Play();
			_crossfadeTimer = 0.0;
			_crossfading = true;
		}
		else
		{
			// Pas de crossfade : on démarre la boucle au volume normal,
			// l'intro continuera jusqu'à sa fin naturelle
			_loopPlayer.VolumeDb = 0f;
			_loopPlayer.Play();
		}
	}

	private void OnIntroFinished()
	{
		// Si la boucle n'a pas encore été déclenchée (OverlapSeconds == 0)
		if (!_loopTriggered)
		{
			_loopTriggered = true;
			StartLoop();
		}
	}

	private void StartLoop()
	{
		if (AudioForLoop is null) return;
		_loopPlayer.Stream = AudioForLoop;
		_loopPlayer.VolumeDb = 0f;
		_loopPlayer.Play();
	}
}
