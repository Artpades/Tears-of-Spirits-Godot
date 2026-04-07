using Godot;
using System;
namespace GameplayToolkit.ObjectMechanics
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.SPRITESWAPPER)]
#endif
	public partial class MeshSwapper : Node
	{

		/// <summary><b>🧊Mesh Échangé</b><br/><br/>📋 Émis après un remplacement réussi de mesh sur le <see cref="MeshInstance3D"/> référencé, avec le <b>nouveau mesh appliqué</b>.<br/><br/>🎓 Connecter pour <b>mettre à jour des collisions</b>, des effets visuels liés, ou notifier d'autres systèmes du changement de forme 3D.</summary><param name="newMesh">🧊 Le nouveau <see cref="Mesh"/> qui vient d'être appliqué sur <see cref="MainMesh3DRef"/>.</param>
		[Signal] public delegate void MeshSwappedEventHandler(Mesh newMesh);
		/// <summary><b>🧊Référence au MeshInstance3D Cible</b><br/><br/>📋 Référence au <see cref="MeshInstance3D"/> dont le mesh sera remplacé par <see cref="SwapMesh"/>.<br/><br/>🎓 Assigner depuis <b>l'inspecteur</b>. Si <c>null</c> lors de l'appel, un avertissement est émis et le swap est annulé.</summary>
		[Export] public MeshInstance3D MainMesh3DRef;


		/// <summary><b>🧊Échange de Mesh sur MeshInstance3D</b><br/><br/>📋 Remplace le mesh de <see cref="MainMesh3DRef"/> par <paramref name="newMesh"/> et émet <see cref="MeshSwapped"/>. <b>Annulé</b> si <see cref="MainMesh3DRef"/> est <c>null</c>.<br/><br/>🎓 Utiliser pour changer la <b>forme 3D</b> d'un objet dynamiquement, comme un changement d'arme, d'armure, ou de LOD manuel.</summary><param name="newMesh">🧊 Le nouveau <see cref="Mesh"/> à appliquer sur <see cref="MainMesh3DRef"/>.</param>
		public void SwapMesh(Mesh newMesh)
		{
			// 🚫 Vérifie que la référence est assignée avant toute modification
			if (MainMesh3DRef is null)
			{
				// ⚠️ Log interne Godot pour alerter dans l'éditeur
				if (DebugTools.LogModeActivated) CustomLogger.Log(Name, IconPaths.SPRITESWAPPER, "[pulse]MainMesh3DRef[/pulse] n'est pas assigné.");

				GD.PushWarning($"[{Name}] MainMesh3DRef n'est pas assigné.");
				return;
			}

			// ✅ Applique le nouveau mesh sur le MeshInstance3D
			MainMesh3DRef.Mesh = newMesh;
			// 📡 Notifie les abonnés avec le mesh effectivement appliqué
			EmitSignal(SignalName.MeshSwapped, MainMesh3DRef.Mesh);
		}
	}
}