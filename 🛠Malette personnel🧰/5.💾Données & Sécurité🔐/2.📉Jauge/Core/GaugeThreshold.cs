using Godot;
using System;
namespace GameplayToolkit.DatasAndPublication;

/// <summary><b>🎯 Seuil de Jauge</b><br/><br/>📋 Représente un <b>seuil en pourcentage</b> utilisé par la classe <see cref="Gauge"/> pour détecter les franchissements de valeurs critiques.<br/><br/>🎓 Utilisable pour définir des points de déclenchement comme les zones critiques de santé (25%), les alertes de ressources basses (50%), ou les états de boost (75%).</summary> <remarks>⚠️ Le pourcentage est automatiquement <b>clampé</b> entre <c>0</c> et <c>100</c>. Les seuils sont évalués par la <see cref="Gauge"/> lors des changements de valeur.</remarks>
[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.GAUGE)]
public partial class GaugeThreshold : Resource
{
    #region 💾 Champs Privés

    /// <summary>Valeur interne du seuil en pourcentage (0-100)</summary>
    private float thresholdPercent;

    #endregion

    #region ⚙️ Propriétés Publiques

    /// <summary> Pourcentage du seuil par rapport à la plage totale de la jauge (Min-Max) </summary> <remarks> La valeur est automatiquement contrainte entre 0 et 100.<br/> Un seuil de 50% représente le point médian entre MinValue et MaxValue de la Gauge. </remarks>
    [Export(PropertyHint.Range, "0,100,0.1")]
    public float ThresholdPercent
    {
        get => thresholdPercent;
        set
        {
            // Garantit que le pourcentage reste dans la plage 0-100
            thresholdPercent = Mathf.Clamp(value, 0f, 100f);
        }
    }

    #endregion
}