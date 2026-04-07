using Godot;
using Godot.Collections;

namespace GameplayToolkit.VisualAndSoundEffects.GUI;

public partial class FancyGUI
{
	[ExportGroup("Fancy Tooltip")]
	[Export] public Tween.TransitionType TooltipTransition = Tween.TransitionType.Back;
	[Export] public Tween.EaseType TooltipEase = Tween.EaseType.Out;
	[Export] public float TooltipAnimationDuration = 0.3f;
	[Export] public Vector2 TooltipOffset = Vector2.Zero;
	[Export] public Vector2 TooltipFocusOffset = new(0, -40f);
	[Export] public Font FontText = new SystemFont();
	[Export] public Color TooltipTextColor = new(1f, 1f, 1f, 1f);

	[Export] public StyleBox TooltipTheme = new StyleBoxFlat();
	[Export] public float TooltipMaxWidth = 400f;
	[Export] public int TooltipFontSize = 14;

	private string _originalTooltipText = "";
	private bool _isTooltipVisible = false;
	private Tween _tooltipTween;

	// Créer une Panel et RichText
	// Position sur le curseur ou sur un offset sur le GUI

	private PanelContainer _tooltipPanel;
	private RichTextLabel _tooltipLabel;

	private void InitializeFancyTooltip()
	{
		_originalTooltipText = FocusParent.TooltipText;

		if (string.IsNullOrEmpty(_originalTooltipText))
			return;

		FocusParent.TooltipText = "";

		_tooltipPanel = new PanelContainer
		{
			Visible = false,
			MouseFilter = Control.MouseFilterEnum.Ignore
		};
		_tooltipPanel.SetAnchorsPreset(Control.LayoutPreset.TopLeft);
		_tooltipPanel.TopLevel = true;
		_tooltipPanel.ZIndex = 1000;
		_tooltipPanel.AddThemeStyleboxOverride("panel", TooltipTheme);

		_tooltipLabel = new RichTextLabel
		{
			BbcodeEnabled = true,
			FitContent = true,
			ScrollActive = false,
			AutowrapMode = TextServer.AutowrapMode.Off,
			// CustomEffects = (Array)ResourceLoader.Load<TextEffectsCollection>(
			// 	"res://addons/🛠️GameplayToolkit🕹️/0.🧰Outils & Éditeur🧑‍💻/0.🔨Utilitaires/Collection text Effets.tres"
			// ).RichEffects
		};
		_tooltipLabel.AddThemeColorOverride("default_color", TooltipTextColor);
		_tooltipLabel.AddThemeFontSizeOverride("normal_font_size", TooltipFontSize);
		_tooltipLabel.Text = _originalTooltipText;

		CalculateOptimalTooltipSize();
		CallDeferred(MethodName.AddTooltipChild);
		_tooltipPanel.Scale = Vector2.Zero;

		// Souris : position dynamique sous le curseur
		FocusParent.MouseEntered += ShowFancyTooltipFromMouse;
		FocusParent.MouseExited += HideFancyTooltip;

		// Clavier / manette : position relative au contrôle parent
		FocusParent.FocusEntered += ShowFancyTooltipFromFocus;
		FocusParent.FocusExited += HideFancyTooltip;
	}

	internal void AddTooltipChild()
	{
		_tooltipPanel.AddChild(_tooltipLabel);

		// Intègre le tooltip comme enfant direct du GUI pour garantir sa visibilité
		FocusParent.AddChild(_tooltipPanel);
	}
	private void UpdateTooltipPosition()
	{
		// Vérifie que le tooltip existe et est actuellement visible
		if (_tooltipPanel == null || !_isTooltipVisible)
			return;

		// Récupère la position actuelle du curseur dans le viewport
		var mousePos = FocusParent.GetViewport().GetCamera2D().GlobalPosition;

		// Applique l'offset configuré pour décaler le tooltip du curseur
		_tooltipPanel.GlobalPosition = mousePos + TooltipOffset;

		// Récupère les dimensions nécessaires pour les ajustements
		var viewportSize = FocusParent.GetViewportRect().Size;
		var tooltipSize = _tooltipPanel.Size;

		// Empêche le tooltip de dépasser à droite de l'écran
		if (_tooltipPanel.GlobalPosition.X + tooltipSize.X > viewportSize.X)
		{
			_tooltipPanel.GlobalPosition = new Vector2(
				viewportSize.X - tooltipSize.X - 10, // 10px de marge
				_tooltipPanel.GlobalPosition.Y
			);
		}

		// Empêche le tooltip de dépasser en bas de l'écran
		if (_tooltipPanel.GlobalPosition.Y + tooltipSize.Y > viewportSize.Y)
		{
			_tooltipPanel.GlobalPosition = new Vector2(
				_tooltipPanel.GlobalPosition.X,
				viewportSize.Y - tooltipSize.Y - 10 // 10px de marge
			);
		}
	}

	/// <summary>
	/// Abonnée à MouseEntered.
	/// Positionne le tooltip à la position actuelle du curseur + TooltipOffset.
	/// </summary>
	private void ShowFancyTooltipFromMouse()
	{
		if (_tooltipPanel == null || _isTooltipVisible)
			return;

		var mousePos = FocusParent.GetViewport().GetCamera2D().GlobalPosition;


		ShowFancyTooltip(mousePos + TooltipOffset);
	}

	/// <summary>
	/// Abonnée à FocusEntered.
	/// Positionne le tooltip relativement au coin supérieur-gauche du contrôle parent
	/// + TooltipFocusOffset (configurable dans l'inspector).
	/// </summary>
	private void ShowFancyTooltipFromFocus()
	{
		if (_tooltipPanel == null || _isTooltipVisible)
			return;

		// Ancre au bord supérieur du contrôle pour rester visible sans curseur souris
		Vector2 controlPos = FocusParent.GlobalPosition + FocusParent.Size;
		ShowFancyTooltip(controlPos + TooltipFocusOffset);
	}

	/// <summary>
	/// Logique d'affichage commune : applique la position, clamp dans le viewport,
	/// et lance l'animation d'apparition.
	/// </summary>
	private void ShowFancyTooltip(Vector2 desiredPosition)
	{
		_isTooltipVisible = true;
		_tooltipPanel.Visible = true;

		// Applique la position demandée
		_tooltipPanel.GlobalPosition = desiredPosition;

		// Clamp dans le viewport
		ClampTooltipInViewport();

		_tooltipTween?.Kill();
		_tooltipTween = FocusParent.CreateTween();
		_tooltipTween
			.TweenProperty(_tooltipPanel, "scale", Vector2.One, TooltipAnimationDuration)
			.SetTrans(TooltipTransition)
			.SetEase(TooltipEase);
	}

	private void HideFancyTooltip()
	{
		if (_tooltipPanel == null || !_isTooltipVisible)
			return;

		_isTooltipVisible = false;

		_tooltipTween?.Kill();
		_tooltipTween = FocusParent.CreateTween();
		_tooltipTween
			.TweenProperty(_tooltipPanel, "scale", Vector2.Zero, TooltipAnimationDuration)
			.SetTrans(TooltipTransition)
			.SetEase(TooltipEase);

		_tooltipTween.TweenCallback(Callable.From(() =>
		{
			if (_tooltipPanel != null)
				_tooltipPanel.Visible = false;
		}));
	}

	/// <summary>
	/// Appelée dans _Process uniquement quand le tooltip est visible via la souris.
	/// </summary>
	public void ProcessTooltip()
	{
		if (string.IsNullOrEmpty(_originalTooltipText) || !_isTooltipVisible)
			return;

		// Pour le focus clavier, la position est fixe : on ne met à jour que si la souris pilote le tooltip
		if (FocusParent.GetViewport().GuiGetFocusOwner() != FocusParent)
		{
			var mousePos = FocusParent.GetViewport().GetCamera2D().GlobalPosition;


			_tooltipPanel.GlobalPosition = mousePos + TooltipOffset;
			ClampTooltipInViewport();
		}
	}

	/// <summary>
	/// Empêche le tooltip de déborder hors du viewport (droite et bas).
	/// </summary>
	private void ClampTooltipInViewport()
	{
		var viewportSize = FocusParent.GetViewportRect().Size;
		var tooltipSize = _tooltipPanel.Size;
		var pos = _tooltipPanel.GlobalPosition;

		if (pos.X + tooltipSize.X > viewportSize.X)
			pos.X = viewportSize.X - tooltipSize.X - 10;

		if (pos.Y + tooltipSize.Y > viewportSize.Y)
			pos.Y = viewportSize.Y - tooltipSize.Y - 10;

		_tooltipPanel.GlobalPosition = pos;
	}
	private void CalculateOptimalTooltipSize()
	{
		// Vérifie que le label existe avant de calculer
		if (_tooltipLabel == null)
			return;

		// Récupère la police utilisée par le label et la taille configurée
		var font = _tooltipLabel.GetThemeFont("normal_font");
		var fontSize = TooltipFontSize;

		// Nettoie le texte des balises BBCode pour obtenir la longueur réelle
		string plainText = RemoveBBCodeTags(_originalTooltipText);

		// Sépare le texte en lignes pour mesurer chaque ligne individuellement
		string[] lines = plainText.Split('\n');
		float maxLineWidth = 0f;

		// Mesure la largeur de chaque ligne pour trouver la plus large
		foreach (string line in lines)
		{
			if (font != null)
			{
				Vector2 textSize = font.GetStringSize(line, HorizontalAlignment.Left, -1, fontSize);
				maxLineWidth = Mathf.Max(maxLineWidth, textSize.X);
			}
		}

		// Active le word wrap si le texte dépasse la largeur maximale autorisée
		if (maxLineWidth > TooltipMaxWidth)
		{
			// Texte trop long : activation du retour à la ligne intelligent
			_tooltipLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
			_tooltipLabel.CustomMinimumSize = new Vector2(TooltipMaxWidth, 0);
		}
		else
		{
			// Texte court : désactivation du wrap pour un tooltip compact
			_tooltipLabel.AutowrapMode = TextServer.AutowrapMode.Off;
			_tooltipLabel.CustomMinimumSize = new Vector2(maxLineWidth + 10, 0); // +10 pour marge de sécurité
		}
	}

	private string RemoveBBCodeTags(string text)
	{
		// Vérifie que le texte n'est pas nul ou vide
		if (string.IsNullOrEmpty(text))
			return text;

		// Utilise Regex pour supprimer toutes les balises [xxx] et [/xxx]
		return MyRegex().Replace(text, "");
	}

	[System.Text.RegularExpressions.GeneratedRegex(@"\[/?[^\]]+\]")]
	internal static partial System.Text.RegularExpressions.Regex MyRegex();
}
