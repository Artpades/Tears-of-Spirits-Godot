using Godot;
using Godot.Collections;
using System;
namespace GameplayToolkit.UIUX;

[GlobalClass,
#if TOOLS
Tool,
#endif
Icon(IconPaths.DIALOGUE)]
public partial class Dialogue : Resource
{
    [Export] public Array<DialogueLine> DialogueTexts;
}
