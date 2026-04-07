using Godot;
using Godot.Collections;

using System.Linq;
namespace GameplayToolkit.VisualAndSoundEffects;

[GlobalClass,
#if TOOLS
Tool,
#endif

]
public partial class PopUpGUI : Control
{
	[Signal] public delegate void PopUpEmittedEventHandler(string text, Vector2 position);
	[Signal] public delegate void FinishedPoppingUpEventHandler();
	[Export] public string TextToDisplay { get; set; } = "[rainbow]Pop Up Text[/rainbow]";
	[Export] public float Duration { get; set; } = 2.0f;
	[Export] public Vector2 InitialVelocity { get; set; } = new Vector2(-1, -1);
	[Export] public Vector2 Gravity { get; set; } = new Vector2(0, 1);
	[Export] public Color TextColor { get; set; } = Colors.White;
	[Export] public int FontSize { get; set; } = 24;
	[Export] public Array<RichTextEffect> TextEffects { get; set; } = [];
	[ExportGroup("Randomization")]
	[Export] public bool AddRNGVelocity { get; set; } = true;
	[Export] public double RNGVelocityRangeX { get; set; } = 1.0f;
#if TOOLS

	[ExportToolButton("PopUp")] public Callable PopupButton => Callable.From(PopUp);
#endif

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (TextEffects.Count == 0 || TextEffects == null)
		{
			// TextEffects = ResourceLoader.Load<TextEffectsCollection>("res://addons/🛠️GameplayToolkit🕹️/0.🧰Outils & Éditeur🧑‍💻/0.🔨Utilitaires/Collection text Effets.tres").RichEffects;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		foreach (var label in GetChildren().OfType<RichTextLabel>())
		{
			var velocity = label.GetMeta("velocity").AsVector2();
			velocity += Gravity;
			label.SetMeta("velocity", velocity);
			label.GlobalPosition += velocity;
		}
	}

	public void PopUp()
	{
		PopUp(TextToDisplay, GlobalPosition);
	}
	public void PopUp(string text, Vector2 position)
	{
		var label = new RichTextLabel
		{
			BbcodeEnabled = true,
			Text = text,
			CustomMinimumSize = new Vector2(200, 50),
			CustomEffects = (Array)TextEffects,
			Modulate = TextColor
		};
		if (AddRNGVelocity)
			label.SetMeta("velocity", InitialVelocity + new Vector2((float)GD.RandRange(-RNGVelocityRangeX, RNGVelocityRangeX), 1f)); // ← vélocité propre à ce label
		else
			label.SetMeta("velocity", InitialVelocity); // ← vélocité propre à ce label

		label.AddThemeFontSizeOverride("normal_font_size", FontSize);

		var timer = new Timer
		{
			WaitTime = Duration,
			OneShot = true,
			Autostart = true
		};
		timer.Timeout += label.QueueFree;

		AddChild(label);
		label.AddChild(timer);

		label.GlobalPosition = position;

		EmitSignal(SignalName.PopUpEmitted, text, position);
		timer.Timeout += () => EmitSignal(SignalName.FinishedPoppingUp);
	}
}
