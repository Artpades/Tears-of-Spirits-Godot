using Godot;
using System;
namespace GameplayToolkit.DatasAndPublication;

/// <summary><b>🎯 Point de Jeu - Ressource de Score</b><br/><br/>📋 Représente une <b>ressource de points</b> utilisée dans le système de jeu. Cette classe hérite de <see cref="Resource"/> et permet de stocker et gérer des valeurs de points avec notification automatique des changements.<br/><br/>🎓 Utilisations possibles :<br/>• Système de score pour le joueur<br/>• Points d'expérience (XP)<br/>• Monnaie virtuelle du jeu<br/>• Compteur de ressources collectables</summary> <remarks>⚠️ Cette classe utilise l'attribut <c>[GlobalClass]</c> pour être accessible dans l'éditeur Godot. Elle émet automatiquement un signal de changement via <c>EmitChanged()</c> lors de la modification des points.</remarks>
[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.GAMEPOINT)]
public partial class GamePoint : Resource
{
    // 🔢 PROPRIÉTÉS ET CHAMPS
    #region 🔢 Propriétés et Champs
    
    /// <summary>Valeur interne des points stockés</summary>
    private int _points;
    
    /// <summary><b>💰 Points - Valeur Exportée</b><br/><br/>📋 Propriété qui représente le <b>nombre de points</b> actuellement stockés dans cette ressource. Accessible et modifiable depuis l'éditeur Godot grâce à l'attribut <c>[Export]</c>.<br/><br/>🎓 Utilisations :<br/>• Lecture du score actuel<br/>• Modification du nombre de points<br/>• Synchronisation automatique avec l'éditeur</summary> <remarks>✨ Lors de la modification de cette propriété, <see cref="EmitChanged"/> est automatiquement appelé pour notifier les observateurs du changement de valeur.</remarks>
    [Export]
    public int Points
    {
        get
        {
            // Retourne la valeur actuelle des points
            return _points;
        }
        set
        {
            // Notifie Godot que la ressource a été modifiée pour mettre à jour l'éditeur
           if(value != _points) EmitChanged();

            // Assigne la nouvelle valeur aux points
            _points = value;
        }
    }
    #endregion
}