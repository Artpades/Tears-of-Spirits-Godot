#if TOOLS
using GameplayToolkit.InputAndControls;
using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class InputButtonActionInspector : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject @object)
    {
        return @object is InputButtonAction;
    }

    public override bool _ParseProperty(GodotObject @object, Variant.Type type, string name, PropertyHint hintType, string hintString, PropertyUsageFlags usageFlags, bool wide)
    {
        //GD.Print(name);

     // if (name == "Key")
     // {
     //     GD.Print(name);
     //     //AddPropertyEditor(name, new RandomIntEditor());
     //    // AddPropertyEditor("Key", new Button() { Text = "Click !" });
     //     // Inform the editor to remove the default property editor for
     //     // this property type.
     //     return true;
     // }
      return false;
    }


}
#endif