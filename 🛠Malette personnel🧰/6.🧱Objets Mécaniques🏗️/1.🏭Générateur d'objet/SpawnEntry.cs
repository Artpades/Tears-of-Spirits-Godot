using Godot;
using Godot.Collections;
using System;

namespace GameplayToolkit.ObjectMechanics
{

#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.SPWANERENTRY)]
#endif
	public partial class SpawnEntry : Resource
	{
		#region 🎛️ Modes de Sélection

		public enum SelectionMode
		{

			Sequential,

			Random,

			All
		}

		#endregion

		#region 📌 Propriétés Exportées

		[Export] public Array<NodePath> Positions;

		[Export] public Array<PackedScene> Scenes;

		[Export] public SelectionMode PosSelectionMode;

		[Export] public SelectionMode ScenesSelectionMode;

		#endregion
	}
}