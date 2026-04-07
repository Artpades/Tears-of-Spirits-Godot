#if TOOLS
using Godot;
namespace GameplayToolkit.Editor
{
    [Tool]
    public partial class GameEventInspector : EditorInspectorPlugin
    {
        public override bool _CanHandle(GodotObject obj) => true; // Tous les objets

        public override bool _ParseProperty(
            GodotObject obj, Variant.Type type, string name,
            PropertyHint hintType, string hintString,
            PropertyUsageFlags usageFlags, bool wide)
        {
            // On intercepte uniquement les propriétés de type GameEvent
            if (type == Variant.Type.Object && hintString == "CustomEvent")
            {
                AddPropertyEditor(name, new CustomEventEditorProperty());
                return true; // On remplace le widget par défaut
            }
            return false;
        }
    }
}
#endif