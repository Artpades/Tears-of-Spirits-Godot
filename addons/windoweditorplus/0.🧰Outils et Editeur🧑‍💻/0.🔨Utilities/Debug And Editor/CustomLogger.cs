using Godot;
using System;

namespace GameplayToolkit;
/// <summary> <b>📰Journal</b><br/><br/>📋Permet d'afficher du texte dans la console.<br/>🎓 Peut être utiliser pour notifier l'utilisation de méthode. </summary>
public static partial class CustomLogger
{
	#region 📝 Débogage et Affichage
	/// <summary> 🗞️Jouranlisation<br/><br/>📋Permet d'afficher du texte dans la console en donnant plus de détaille sur quel objet l'utilise.<br/>🎓 Il suffit simplement de l'appeler dans une <b>méthode/Evènement/Propriété/Constructor/etc...</b><br/></summary> <param name="className">Nom de la classe qui l'utilise</param> <param name="iconPath">Chemin d'icône pour différencier les journaux par la vue</param> <param name="customMessage">Un Message personnalisé</param>
	public static void Log(string className, string iconPath, string customMessage = "[Message non spécifié !]")
	{
		// Affichage enrichi combinant le nom du nœud, une icône et un message personnalisé
		GD.PrintRich($"{className} [img]{iconPath}[/img] : {customMessage}");
	}
	#endregion
}