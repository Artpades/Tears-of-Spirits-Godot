using GameplayToolkit.Cameras;
using Godot;
using System;
namespace GameplayToolkit.ObjectInteractionAndInfluencer
{

	/// <summary><b>🎨Échangeur Visuel de Ressources</b><br/><br/>📋 Nœud utilitaire permettant de <b>remplacer dynamiquement</b> la texture d'un <see cref="Polygon2D"/>, d'un <see cref="Sprite2D"/>, ou le mesh d'un <see cref="MeshInstance3D"/> à l'exécution.<br/><br/>🎓 Utile pour des <b>changements d'apparence</b> en cours de jeu : changement de skin, d'état visuel, d'équipement affiché. Connecter <see cref="TextureSwapped"/> ou <see cref="MeshSwapped"/> pour réagir aux échanges.</summary>
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.SPRITESWAPPER)]
#endif
	public partial class VisualSwapper : Node
	{

		/// <summary><b>🖼️Texture Échangée</b><br/><br/>📋 Émis après un remplacement réussi de texture sur un <see cref="Polygon2D"/> ou un <see cref="Sprite2D"/>, avec la <b>nouvelle texture appliquée</b>.<br/><br/>🎓 Connecter pour <b>synchroniser d'autres visuels</b> ou déclencher un effet après le swap.</summary><param name="newTexture">🖼️ La nouvelle <see cref="Texture2D"/> qui vient d'être appliquée sur le visuel cible.</param>
		[Signal] public delegate void TextureSwappedEventHandler(Texture2D newTexture);


		/// <summary><b>🔷Référence au Polygon2D Cible</b><br/><br/>📋 Référence au <see cref="Polygon2D"/> dont la texture sera remplacée par <see cref="SwapTexturePolygon"/>.<br/><br/>🎓 Assigner depuis <b>l'inspecteur</b>. Si <c>null</c> lors de l'appel, un avertissement est émis et le swap est annulé.</summary>
		[Export] public Polygon2D MainPolygonRef;


		/// <summary><b>🔷Échange de Texture sur Polygon2D</b><br/><br/>📋 Remplace la texture de <see cref="MainPolygonRef"/> par <paramref name="newTexture"/> et émet <see cref="TextureSwapped"/>. <b>Annulé</b> si <see cref="MainPolygonRef"/> est <c>null</c>.<br/><br/>🎓 Utiliser pour changer l'apparence d'une <b>forme 2D polygonale</b> texturée dynamiquement.</summary> <param name="newTexture">🖼️ La nouvelle <see cref="Texture2D"/> à appliquer sur <see cref="MainPolygonRef"/>.</param>
		public void SwapTexturePolygon(Texture2D newTexture)
		{
			// 🚫 Vérifie que la référence est assignée avant toute modification
			if (MainPolygonRef is null)
			{
				// ⚠️ Log interne Godot pour alerter dans l'éditeur
				if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SPRITESWAPPER, "[pulse]MainPolygonRef[/pulse] n'est pas assigné.");

				GD.PushWarning($"[{nameof(VisualSwapper)}] MainPolygonRef n'est pas assigné.");
				return;
			}

			// ✅ Applique la nouvelle texture sur le Polygon2D
			MainPolygonRef.Texture = newTexture;
			// 📡 Notifie les abonnés avec la texture effectivement appliquée
			EmitSignal(SignalName.TextureSwapped, MainPolygonRef.Texture);
		}


	}
}