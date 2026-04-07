using Godot;
using Godot.Collections;
namespace GameplayToolkit
{
#if TOOLS
    [GlobalClass, Tool, Icon(IconPaths.SIGNALEMITTER)]
#endif
    public partial class CustomEvent : Resource
    {
        // Liste des cibles et méthodes enregistrées
        [Export] public Array<string> Targets { get; set; } = [];
        [Export] public Array<string> Methods { get; set; } = [];

        public void Invoke()
        {
            for (int i = 0; i < Targets.Count; i++)
            {
                var node = TypeSearcher.GetNodeByName(Targets[i], TypeSearcher.GetRoot(), true);
                if (node != null && node.HasMethod(Methods[i]))
                    node.Call(Methods[i]);
            }
        }
    }
}
