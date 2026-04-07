using Godot;
using System;
namespace GameplayToolkit.VisualAndSoundEffects.GUI;

public partial class FancyGUI
{
    [ExportGroup("Changement de taille")]
    [Export] public bool SizeChangeActivated;
    [Export] Vector2 SizeOnHover = new(1.2f, 1.2f);
    [Export] Vector2 SizeOnUnhover = new(1.0f, 1.0f);
    [Export] Tween.TransitionType Transition = Tween.TransitionType.Back;
    [Export] Tween.EaseType Ease = Tween.EaseType.Out;
    [Export] float LerpSizeDuration = 0.5f;

    private void InitSizeChangeMode()
    {
        // Connecte les événements de focus pour déclencher l'animation.
        FocusParent.FocusEntered += Hovered;
        FocusParent.FocusExited += Unhovered;

        // Connecte les événements de souris pour déclencher l'animation.
        FocusParent.MouseEntered += Hovered;
        FocusParent.MouseExited += Unhovered;

    }
    public void Hovered()
    {
        // Crée un tween pour animer la mise à l'échelle vers la taille de survol.
        Tween tween = FocusParent.CreateTween();
        tween.TweenProperty(FocusParent, "scale", SizeOnHover, LerpSizeDuration)
        .SetTrans(Transition) // Applique la courbe de transition configurée.
        .SetEase(Ease);       // Applique le mode d'accélération configuré.
    }

    public void Unhovered()
    {
        // Crée un tween pour revenir à la taille d'origine.
        Tween tween = FocusParent.CreateTween();
        tween.TweenProperty(FocusParent, "scale", SizeOnUnhover, LerpSizeDuration)
        .SetTrans(Transition) // Applique la courbe de transition configurée.
        .SetEase(Ease);       // Applique le mode d'accélération configuré.
    }

}
