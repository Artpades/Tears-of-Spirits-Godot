#if TOOLS
using Godot;
using Godot.Collections;
using System;

namespace GameplayToolkit.ToolsAndEdit
{
    [Tool, Icon(IconPaths.ITEMTECHTREE)]
    public partial class TTItemEditor : Panel
    {
		[ExportCategory("Paramètres")]
		[Export] public string ItemName { get; set; } = "Tech Tree Item";

		[ExportCategory("Références")]
		[Export] private Label _titleLabel;
		[Export] private Button _InputButton;
		[Export] private Button _OutputButton;
		[Export] private Button _ResizeButton;

        public override void _Ready()
		{
			_titleLabel.Text = ItemName;
			_InputButton.Pressed += () => GD.Print($"Input port of {ItemName} pressed !");
			_OutputButton.Pressed += () => GD.Print($"Output port of {ItemName} pressed !");
			_ResizeButton.Pressed += () =>
			{
				///RectSize += new Vector2(20, 20);
				//GD.Print($"Resized {ItemName} to {RectSize}");
			};
			// TODO : drag and drop pour déplacer l'item dans l'éditeur	
		}

        public override void _Process(double delta)
        {
			_titleLabel.Text = ItemName; // Permet de voir le changement de nom en temps réel dans l'éditeur
			// TODO : mettre à jour la position des ports d'entrée/sortie en fonction de la taille de l'item
			// TODO : gérer les connexions visuellement (lignes entre les ports) et en données (liste de TTConnection dans TechTreeEditor)
			// TODO : ajouter un menu contextuel pour supprimer l'item, ajouter des ports, etc.
			// TODO : ajouter des propriétés personnalisées pour chaque item (coût, description, etc.) et les afficher dans l'éditeur
        }


    }
}
#endif