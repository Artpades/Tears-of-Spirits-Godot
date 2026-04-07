using Godot;
using System;
namespace GameplayToolkit.ObjectMechanics
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.SPRITESWAPPER)]
#endif

	public partial class SpriteSwapper : Node
	{
		/// <summary><b>🖼️Texture Échangée</b><br/><br/>📋 Émis après un remplacement réussi de texture sur un <see cref="Polygon2D"/> ou un <see cref="Sprite2D"/>, avec la <b>nouvelle texture appliquée</b>.<br/><br/>🎓 Connecter pour <b>synchroniser d'autres visuels</b> ou déclencher un effet après le swap.</summary><param name="newTexture">🖼️ La nouvelle <see cref="Texture2D"/> qui vient d'être appliquée sur le visuel cible.</param>
		[Signal] public delegate void TextureSwappedEventHandler(Texture2D newTexture);

		/// <summary><b>🔸Référence au Sprite2D Cible</b><br/><br/>📋 Référence au <see cref="Sprite2D"/> dont la texture sera remplacée par <see cref="SwapTextureForSprite"/>.<br/><br/>🎓 Assigner depuis <b>l'inspecteur</b>. Si <c>null</c> lors de l'appel, un avertissement est émis et le swap est annulé.</summary>
		[Export] public Sprite2D MainSprite2DRef;

		/// <summary><b>🔸Échange de Texture sur Sprite2D</b><br/><br/>📋 Remplace la texture de <see cref="MainSprite2DRef"/> par <paramref name="newTexture"/> et émet <see cref="TextureSwapped"/>. <b>Annulé</b> si <see cref="MainSprite2DRef"/> est <c>null</c>.<br/><br/>🎓 Utiliser pour changer le <b>sprite d'un personnage</b> ou d'un objet 2D à la volée, par exemple lors d'un changement d'état ou d'équipement.</summary><param name="newTexture">🖼️ La nouvelle <see cref="Texture2D"/> à appliquer sur <see cref="MainSprite2DRef"/>.</param>
		public void SwapTextureForSprite(Texture2D newTexture)
		{
			// 🚫 Vérifie que la référence est assignée avant toute modification
			if (MainSprite2DRef is null)
			{
				// ⚠️ Log interne Godot pour alerter dans l'éditeur
				if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SPRITESWAPPER, "[pulse]MainSprite2DRef[/pulse] n'est pas assigné.");

				GD.PushWarning($"[{Name}] MainSprite2DRef n'est pas assigné.");
				return;
			}
			// ✅ Applique la nouvelle texture sur le Sprite2D
			MainSprite2DRef.Texture = newTexture;
			// 📡 Notifie les abonnés avec la texture effectivement appliquée
			EmitSignal(SignalName.TextureSwapped, MainSprite2DRef.Texture);
		}

	}

}
