using Godot;
using System;
namespace GameplayToolkit.UIUX;

[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.EMPTYDIALOGUE)]
public partial class DialogueLine : Resource
{
    [Export(PropertyHint.MultilineText)] public string TextLine;
}
