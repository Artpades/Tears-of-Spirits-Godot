#if TOOLS
using Godot;
using System;
namespace GameplayToolkit.Editor
{
    [GlobalClass, Tool]
    public partial class CustomEditorOpenner : Resource
    {
        [ExportToolButton("Vérifier si les gestionnaire existe dans la scène")]public Callable ManagerCheckerButton => Callable.From(OpenCheckerWindow);
        internal void OpenCheckerWindow()
        {
            GD.PushWarning("Work In Progress !!!");
        }
    }

}
#endif