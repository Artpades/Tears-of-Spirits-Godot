#if TOOLS
using Godot;
using Godot.Collections;
using System.Linq;

namespace GameplayToolkit.Editor
{
    /// <summary>
    /// Popup qui liste les méthodes sans paramètre et retournant void
    /// disponibles sur un Node donné (via ClassDB).
    /// </summary>
    [Tool]
    public partial class MethodPickerPopup : PopupPanel
    {
        [Signal] public delegate void MethodSelectedEventHandler(string methodName);

        private VBoxContainer _list;
        private LineEdit _searchBox;

        // Cache : méthodes disponibles pour le nœud courant
        private System.Collections.Generic.List<string> _methods = [];

        public override void _Ready()
        {
            MinSize = new Vector2I(280, 320);

            var root = new VBoxContainer();
            root.AddThemeConstantOverride("separation", 4);
            AddChild(root);

            // Barre de recherche
            _searchBox = new LineEdit { PlaceholderText = "Filtrer les méthodes..." };
            _searchBox.TextChanged += _ => FilterList(_searchBox.Text);
            root.AddChild(_searchBox);

            var scroll = new ScrollContainer { SizeFlagsVertical = Control.SizeFlags.ExpandFill };
            root.AddChild(scroll);

            _list = new VBoxContainer();
            _list.AddThemeConstantOverride("separation", 2);
            scroll.AddChild(_list);
        }

        /// <summary>Ouvre le popup et peuple la liste des méthodes compatibles.</summary>
        public void Open(Node targetNode, Vector2 position)
        {
            _methods.Clear();
            _searchBox.Text = "";

            string className = targetNode.GetClass();
            FetchCompatibleMethods(className);

            FilterList("");
            PopupOnParent(new Rect2I((Vector2I)position, new Vector2I(280, 320)));
        }

        // ──────────────────────────────────────────────────────
        //  Peuplement
        // ──────────────────────────────────────────────────────

        private void FetchCompatibleMethods(string className)
        {
            // ClassDB.ClassGetMethodList retourne aussi les méthodes héritées
            var methodList = ClassDB.ClassGetMethodList(className, noInheritance: false);

            foreach (var methodDict in methodList)
            {
                string name = methodDict["name"].AsString();

                // Filtre : on veut uniquement les méthodes void sans paramètre
                // return_val.type == 0 → Variant.Type.Nil → void
                // args est vide
                var returnVal = methodDict["return"].AsGodotDictionary();
                var args = methodDict["args"].AsGodotArray();

                bool isVoid = (int)returnVal["type"] == (int)Variant.Type.Nil;
                bool noParams = args.Count == 0;

                // On ignore les méthodes internes Godot (commençant par _)
                // mais on les inclut si elles sont explicitement overridées
                bool isPrivate = name.StartsWith("_");

                if (isVoid && noParams && !isPrivate)
                    _methods.Add(name);
            }

            _methods.Sort();
        }

        private void FilterList(string filter)
        {
            foreach (Node child in _list.GetChildren())
                child.QueueFree();

            var filtered = string.IsNullOrEmpty(filter)
                ? _methods
                : _methods.Where(m => m.Contains(filter, System.StringComparison.OrdinalIgnoreCase)).ToList();

            if (filtered.Count == 0)
            {
                var empty = new Label
                {
                    Text = "Aucune méthode compatible.",
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                empty.AddThemeColorOverride("font_color", new Color(0.5f, 0.5f, 0.5f));
                _list.AddChild(empty);
                return;
            }

            foreach (string methodName in filtered)
            {
                string captured = methodName;
                var btn = new Button
                {
                    Text = captured,
                    Alignment = HorizontalAlignment.Left,
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                    Flat = true  // → plus discret, style liste
                };
                btn.Pressed += () =>
                {
                    EmitSignal(SignalName.MethodSelected, captured);
                    Hide();
                };
                _list.AddChild(btn);
            }
        }
    }
}
#endif