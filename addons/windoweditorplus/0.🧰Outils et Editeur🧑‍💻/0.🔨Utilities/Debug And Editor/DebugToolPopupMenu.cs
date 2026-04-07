using Godot;
using System;
namespace GameplayToolkit
{
#if TOOLS
	[Tool]
#endif
	public partial class DebugToolPopupMenu : PopupMenu
	{
		#region Popups disponibles
		PopupMenu subMenuToolEdit = new();
		PopupMenu subMenuInputControls = new();
		PopupMenu subMenuCams = new();
		PopupMenu subMenuCharactersAlgos = new();
		PopupMenu subMenuMovementPhys = new();
		PopupMenu subMenuDatasPublications = new();
		PopupMenu subMenuObjMechas = new();
		PopupMenu subMenuGameplaySystems = new();
		PopupMenu subMenuUIUX = new();
		PopupMenu subMenuEnvironmentNetWork = new();
		PopupMenu subMenuFX = new();
		#endregion

		public override void _EnterTree()
		{
			Name = "PopUpForLog";

			#region Créations des Catégories

			AddSubmenuNodeItem("🧰Outils & Editeur", subMenuToolEdit, ID.TE);
			AddSubmenuNodeItem("🎮Input & Contrôles", subMenuInputControls, ID.IC);
			AddSubmenuNodeItem("🎥Caméras", subMenuCams, ID.C);
			AddSubmenuNodeItem("🧍Personnage & Algorithme🧠", subMenuCharactersAlgos, ID.CA);
			AddSubmenuNodeItem("🌀Mouvement & Physique", subMenuMovementPhys, ID.MP);
			AddSubmenuNodeItem("💾Données & Publication💰", subMenuDatasPublications, ID.DP);
			AddSubmenuNodeItem("🧱Objets & Mécaniques⏱️", subMenuObjMechas, ID.OM);
			AddSubmenuNodeItem("⚔️Systèmes de Gameplay", subMenuGameplaySystems, ID.GS);
			AddSubmenuNodeItem("🧭UI & UX", subMenuUIUX, ID.UIX);
			AddSubmenuNodeItem("🌍Environnement & Réseau🌐", subMenuEnvironmentNetWork, ID.EN);
			AddSubmenuNodeItem("✨Effets🎵", subMenuFX, ID.FX);

			#endregion

			#region Créations des Sous-catégories

			#region 🧰

			subMenuToolEdit.AddIconCheckItem(Q(IconPaths.WINDOW), "CustomWindow", ID.WINDOWEDITOR);
			#endregion

			#region 🎮

			subMenuInputControls.AddIconCheckItem(Q(IconPaths.INPUTMANAGER), "Input Managers", ID.INPUTMANAGERS);
			subMenuInputControls.AddIconCheckItem(Q(IconPaths.INPUTACTION), "Input Actions", ID.INPUTACTIONS);

			#endregion

			#region 🎥
			#endregion

			#region 🧍🧠
			#endregion

			#region 🌀
			#endregion

			#region 💾💰
			#endregion

			#region 🧱⏱️
			#endregion

			#region ⚔️
			#endregion

			#region 🧭
			#endregion

			#region 🌍🌐
			#endregion

			#region ✨🎵
			#endregion

			#endregion

			#region Assignations des signaux
			subMenuToolEdit.IdPressed += (id) => OnIdPressedToolEdits(id, subMenuToolEdit);
			subMenuInputControls.IdPressed += (id) => OnIdPressedInputControls(id, subMenuInputControls);

			#endregion
		}

		#region Callback
		#region 🧰

		private void OnIdPressedToolEdits(long id, PopupMenu subMenuTarget)
		{
			if (id == ID.WINDOWEDITOR) DebugTools.LogWindowEditor = L(subMenuTarget, (int)id, "CustomWindow", IconPaths.WINDOW);

		}
		#endregion

		#region 🎮

		private void OnIdPressedInputControls(long id, PopupMenu subMenuTarget)
		{
			if (id == ID.INPUTMANAGERS) DebugTools.LogInputManagers = L(subMenuTarget, (int)id, "Input Managers", IconPaths.INPUTMANAGER);
			if (id == ID.INPUTACTIONS) DebugTools.LogInputActions = L(subMenuTarget, (int)id, "Input Actions", IconPaths.INPUTACTION);
		}
		#endregion

		#region 🎥
		#endregion

		#region 🧍🧠
		#endregion

		#region 🌀
		#endregion

		#region 💾💰
		#endregion

		#region 🧱⏱️
		#endregion

		#region ⚔️
		#endregion

		#region 🧭
		#endregion

		#region 🌍🌐
		#endregion

		#region ✨🎵
		#endregion
		#endregion

		// Pour rapidement récupérer une image
		internal static Texture2D Q(string iconPath) => ResourceLoader.Load<Texture2D>(iconPath);
		// Pour log rapidement l'état du DebugTool spécifique
		internal bool L(PopupMenu subMenuTarget, long id, string moduleName, string iconPath)
		{
			var logStateToChange = !subMenuTarget.IsItemChecked((int)id);
			subMenuTarget.SetItemChecked((int)id, logStateToChange);

			if (logStateToChange) CustomLogger.Log(Name, IconPaths.PADLOCK, $"Log pour [color=green]{moduleName}[/color] de l'index [color=cyan]{id}[/color] [img]{iconPath}[/img] est à : [rainbow]{logStateToChange}[/rainbow]");
			else CustomLogger.Log(Name, IconPaths.PADLOCK, $"Log pour [color=green]{moduleName}[/color] de l'index [color=cyan]{id}[/color] [img]{iconPath}[/img] est à : [color=dark_red]{logStateToChange}[/color]");

			return logStateToChange;
		}

		#region ID des sélectionnables
		internal class ID
		{
			public const int TE = 0;
			public const int IC = 1;
			public const int C = 2;
			public const int CA = 3;
			public const int MP = 4;
			public const int DP = 5;
			public const int OM = 6;
			public const int GS = 7;
			public const int UIX = 8;
			public const int EN = 9;
			public const int FX = 10;

			#region 🧰
			public const int WINDOWEDITOR = 0;
			#endregion

			#region 🎮
			public const int INPUTMANAGERS = 0;
			public const int INPUTACTIONS = 1;

			#endregion

			#region 🎥
			#endregion

			#region 🧍🧠
			#endregion

			#region 🌀
			#endregion

			#region 💾💰
			#endregion

			#region 🧱⏱️
			#endregion

			#region ⚔️
			#endregion

			#region 🧭
			#endregion

			#region 🌍🌐
			#endregion

			#region ✨🎵
			#endregion
		}

		#endregion
	}
}