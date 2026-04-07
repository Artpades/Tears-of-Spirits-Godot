#if TOOLS
using Godot;

namespace GameplayToolkit.InputAndControls.Editor
{
	/// <summary>
	/// Popup simple qui propose de choisir entre Button, Axis et Circle.
	/// Émet ActionTypeChosen(string typeName) quand l'utilisateur choisit.
	/// </summary>
	[Tool]
	public partial class AddActionTypePopup : PopupPanel
	{
		[Signal] public delegate void ActionTypeChosenEventHandler(string typeName);

		public override void _Ready()
		{
			var vbox = new VBoxContainer();
			AddChild(vbox);

			var label = new Label
			{
				Text = "Choisir le type d'action :",
				HorizontalAlignment = HorizontalAlignment.Center
			};
			vbox.AddChild(label);
			vbox.AddChild(new HSeparator());

			AddTypeButton(vbox, "🔘 Button Action", "Button", "Pression / relâchement d'un bouton");
			AddTypeButton(vbox, "↔ Axis Action", "Axis", "Valeur float entre -1 et 1");
			AddTypeButton(vbox, "🕹 Circle Action", "Circle", "Vecteur 2D (joystick / WASD)");
		}

		private void AddTypeButton(VBoxContainer parent, string label, string typeName, string tooltip)
		{
			var btn = new Button
			{
				Text = label,
				TooltipText = tooltip,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};
			btn.Pressed += () =>
			{
				EmitSignal(SignalName.ActionTypeChosen, typeName);
				Hide();
			};
			parent.AddChild(btn);
		}
	}
}
#endif