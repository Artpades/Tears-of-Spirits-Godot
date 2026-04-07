using Godot;
using System;
namespace GameplayToolkit.DatasAndPublication;

/// <summary><b>📊 Jauge de Valeur Contrôlée</b><br/><br/>📋 Représente une <b>jauge</b> avec une valeur contrainte entre un <c>minimum</c> et un <c>maximum</c>. Cette ressource émet automatiquement un signal de changement lorsque la valeur est modifiée.<br/><br/>🎓 Utilisable pour créer des barres de vie, barres de mana, jauges d'endurance, compteurs de ressources, ou tout système nécessitant une valeur bornée avec notification de changement.</summary>  <remarks>⚠️ La valeur est automatiquement <b>clampée</b> entre <see cref="MinValue"/> et <see cref="MaxValue"/>. Le signal <c>CurrentValueChanged</c> est émis uniquement si la nouvelle valeur diffère de l'ancienne.</remarks>
[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.GAUGE)]
public partial class Gauge : Resource
{
    #region 📡 Signaux

    /// <summary>Signal émis lorsque la valeur minimale change</summary><param name="newValue">Nouvelle valeur minimale</param>
    [Signal] public delegate void MinValueChangedEventHandler(float newValue);

    /// <summary>Signal émis lorsque la valeur courante change</summary><param name="newValue">Nouvelle valeur courante</param>
    [Signal] public delegate void CurrentValueChangedEventHandler(float newValue);

    /// <summary>Signal émis lorsque la valeur maximale change</summary><param name="newValue">Nouvelle valeur maximale</param>
    [Signal] public delegate void MaxValueChangedEventHandler(float newValue);

    /// <summary>Signal émis lorsqu'un seuil est franchi vers le haut (valeur croissante)</summary><param name="ThresholdPercent">Pourcentage du seuil franchi</param>
    [Signal] public delegate void ThresholdCrossUpEventHandler(float ThresholdPercent);

    /// <summary>Signal émis lorsqu'un seuil est franchi vers le bas (valeur décroissante)</summary><param name="ThresholdPercent">Pourcentage du seuil franchi</param>
    [Signal] public delegate void ThresholdCrossDownEventHandler(float ThresholdPercent);

    #endregion

    #region 💾 Champs Privés

    /// <summary>Valeur actuelle de la jauge</summary>
    private float currentValue = 100;

    /// <summary>Valeur précédente de la jauge (utilisée pour la détection de franchissement de seuils)</summary>
    private float previousValue;

    /// <summary>Valeur minimale autorisée pour la jauge</summary>
    private float minValue;

    /// <summary>Valeur maximale autorisée pour la jauge</summary>
    private float maxValue = 100;

    #endregion

    #region ⚙️ Propriétés Publiques

    /// <summary>Valeur courante de la jauge</summary> <remarks>La valeur est automatiquement contrainte entre <see cref="MinValue"/> et <see cref="MaxValue"/>.<br/>Émet le signal <see cref="CurrentValueChanged"/> et vérifie les franchissements de seuils si la valeur change.</remarks>
    [Export]
    public float CurrentValue
    {
        get => currentValue;
        set
        {
            // Limite la valeur entre les bornes min et max
            float clampedValue = Mathf.Clamp(value, MinValue, MaxValue);

            // Vérifie si la valeur a réellement changé
            if (currentValue != clampedValue)
            {
                // Sauvegarde l'ancienne valeur pour la détection de seuils
                previousValue = currentValue;
                currentValue = clampedValue;

                // Émet le signal de changement de valeur
                EmitSignal(nameof(CurrentValueChanged), currentValue);

                // Vérifie si des seuils ont été franchis
                CheckThresholds();

                // Log de débogage avec calcul du pourcentage
                float percentage = (currentValue - MinValue) / (MaxValue - MinValue) * 100f;
                if (DebugTools.LogModeActivated) CustomLogger.Log(ResourceName, IconPaths.GAUGE,
                $"[color=cyan]📊 Valeur actuelle modifiée: {previousValue:F1} → {currentValue:F1} ({percentage:F1}%)[/color]");
            }
        }
    }

    /// <summary>Valeur minimale de la jauge</summary> <remarks>Si la nouvelle valeur dépasse <see cref="MaxValue"/>, MaxValue sera ajustée automatiquement.<br/><see cref="CurrentValue"/> sera recalculée pour respecter les nouvelles limites.<br/>Émet le signal <see cref="MinValueChanged"/>.</remarks>
    [Export]
    public float MinValue
    {
        get => minValue;
        set
        {
            // Sauvegarde l'ancienne valeur pour le log
            float oldMin = minValue;
            minValue = value;

            // Garantit que MinValue ne dépasse jamais MaxValue
            if (minValue > maxValue)
                maxValue = minValue;

            // Reclamp la valeur courante avec les nouvelles limites
            CurrentValue = Mathf.Clamp(currentValue, minValue, maxValue);

            // Émet le signal de changement
            EmitSignal(nameof(MinValueChanged), minValue);

            // Log de débogage
            if (DebugTools.LogModeActivated) CustomLogger.Log(ResourceName, IconPaths.GAUGE,
            $"[color=orange]📉 Valeur minimale modifiée: {oldMin:F1} → {minValue:F1}[/color]");
        }
    }

    /// <summary>Valeur maximale de la jauge</summary> <remarks>Si la nouvelle valeur est inférieure à <see cref="MinValue"/>, MinValue sera ajustée automatiquement.<br/><see cref="CurrentValue"/> sera recalculée pour respecter les nouvelles limites.<br/>Émet le signal <see cref="MaxValueChanged"/>.</remarks>
    [Export]
    public float MaxValue
    {
        get => maxValue;
        set
        {
            // Sauvegarde l'ancienne valeur pour le log
            float oldMax = maxValue;
            maxValue = value;

            // Garantit que MaxValue ne soit jamais inférieure à MinValue
            if (maxValue < minValue)
                minValue = maxValue;

            // Reclamp la valeur courante avec les nouvelles limites
            CurrentValue = Mathf.Clamp(currentValue, minValue, maxValue);

            // Émet le signal de changement
            EmitSignal(nameof(MaxValueChanged), maxValue);

            // Log de débogage
            if (DebugTools.LogModeActivated) CustomLogger.Log(ResourceName, IconPaths.GAUGE,
            $"[color=green]📈 Valeur maximale modifiée: {oldMax:F1} → {maxValue:F1}[/color]");
        }
    }

    /// <summary>Tableau de seuils à surveiller pour les franchissements</summary> <remarks>Les seuils sont évalués à chaque modification de <see cref="CurrentValue"/>.<br/>Émet <see cref="ThresholdCrossUp"/> ou <see cref="ThresholdCrossDown"/> selon le sens du franchissement.</remarks>
    [Export] public GaugeThreshold[] Thresholds;

    #endregion


    #region 🎯 Système de Seuils

    /// <summary>Vérifie si des seuils ont été franchis lors du dernier changement de valeur</summary> <remarks>Compare <see cref="previousValue"/> et <see cref="currentValue"/> pour détecter les franchissements.<br/>Émet les signaux <see cref="ThresholdCrossUp"/> ou <see cref="ThresholdCrossDown"/> selon le cas.</remarks>
    private void CheckThresholds()
    {
        // Vérifie que le tableau de seuils existe et contient des éléments
        if (Thresholds == null || Thresholds.Length == 0)
            return;

        // Parcourt chaque seuil défini
        foreach (var threshold in Thresholds)
        {
            // Ignore les références nulles
            if (threshold == null)
                continue;

            // Calcul de la valeur absolue du seuil en fonction de la plage Min-Max
            float thresholdValue = MinValue + threshold.ThresholdPercent / 100f * (MaxValue - MinValue);

            // Détection du franchissement vers le haut (ascendant)
            if (previousValue < thresholdValue && currentValue >= thresholdValue)
            {
                // Émet le signal de franchissement ascendant
                EmitSignal(nameof(ThresholdCrossUp), threshold.ThresholdPercent);

                // Log de débogage pour le franchissement montant
                if (DebugTools.LogModeActivated) CustomLogger.Log(ResourceName, IconPaths.GAUGE,
                $"[color=yellow]⬆️ Seuil franchi vers le haut: {threshold.ThresholdPercent}% (Valeur: {thresholdValue:F1})[/color]");
            }
            // Détection du franchissement vers le bas (descendant)
            else if (previousValue > thresholdValue && currentValue <= thresholdValue)
            {
                // Émet le signal de franchissement descendant
                EmitSignal(nameof(ThresholdCrossDown), threshold.ThresholdPercent);

                // Log de débogage pour le franchissement descendant
                if (DebugTools.LogModeActivated) CustomLogger.Log(ResourceName, IconPaths.GAUGE,
                $"[color=magenta]⬇️ Seuil franchi vers le bas: {threshold.ThresholdPercent}% (Valeur: {thresholdValue:F1})[/color]");
            }
        }
    }

    #endregion

}