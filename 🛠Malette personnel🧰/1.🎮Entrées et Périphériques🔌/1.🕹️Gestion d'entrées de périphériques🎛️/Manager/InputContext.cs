using Godot;
using System;
namespace GameplayToolkit.InputAndControls
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.INPUTCONTEXT)]
#endif
	public partial class InputContext : Node
	{
		private bool _contextActivated = true;
		[Export]
		public bool ContextActivated
		{
			get => _contextActivated;
			set
			{
				if (_contextActivated != value)
				{
					_contextActivated = value;

					ProcessMode = _contextActivated ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;

					if (DebugTools.LogInputManagers)
						CustomLogger.Log(Name, IconPaths.INPUTCONTEXT,
							_contextActivated
								? "[color=green]Contexte activé[/color] — ProcessMode : [b]Inherit[/b]"
								: "[color=red]Contexte désactivé[/color] — ProcessMode : [b]Disabled[/b]");

				}
			}
		}

	}
}