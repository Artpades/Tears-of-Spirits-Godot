namespace GameplayToolkit;

using Godot;
using Godot.Collections;
public static partial class TypeSearcher
{
	/// <summary> <c>🔎 Chercheur de Nœud</c><br/> Parcourt l'intégralité de l'arborescence enfant du nœud fourni et retourne tous les nœuds correspondant au type <c>T</c>.<br/><br/> </summary> <typeparam name="T">Le type d'objet rechercher. Insérer le nom de type (doit hériter de <see cref="Node"/>).</typeparam> <param name="root">Nœud racine à partir duquel la recherche récursive est effectuée.</param> <returns>Un <c>Tableau</c> contenant tous les nœuds trouvés correspondant au type <c>T</c>.</returns>
	public static Array<T> GetAllNodeOfType<[MustBeVariant] T>(bool logIt = false) where T : Node
	{
		Array<Node> allNodes = GetRoot().FindChildren("*", typeof(T).Name, owned: false);
		Array<T> nodeFound = [];

		foreach (Node node in allNodes)
		{
			if (node is T typedNode)
			{
				nodeFound.Add(typedNode);
				if (logIt) CustomLogger.Log("[[rainbow]TypeSearcher[/rainbow]]", IconPaths.PARAMMETHODBINDING, $" trouvé : {typedNode.Name}");
			}
		}

		return nodeFound;
	}

	public static T GetNodeByNameOfType<[MustBeVariant] T>(string name, Node root, bool logIt = false, bool isRoot = true) where T : Node
	{
		for (int i = 0; i < root.GetChildCount(true); i++)
		{
			Node child = root.GetChild(i, true);

			if (child.Name == name)
			{
				if (logIt) CustomLogger.Log("[[rainbow]TypeSearcher[/rainbow]]", IconPaths.PARAMMETHODBINDING, $" [wave]{child.Name}[/wave] : trouvé");
				return child as T;
			}

			T found = GetNodeByNameOfType<T>(name, child, logIt, isRoot: false);
			if (found != null) return found;
		}

		if (isRoot && logIt) CustomLogger.Log("[[pulse]TypeSearcher[/pulse]]", IconPaths.PARAMMETHODBINDING, $" Noeud introuvable");
		return null;
	}

	/// <summary><b>🔒Récursion Interne de Recherche par Nom</b><br/><br/>📋 Implémentation récursive <b>privée</b> du chercheur par nom. Parcourt les enfants du nœud fourni et retourne le premier correspondant à <paramref name="name"/>.<br/><br/>🎓 Ne pas appeler directement. Utiliser <see cref="GetNodeByName"/> comme point d'entrée public.</summary><param name="name">🏷️ Nom exact du nœud recherché, sensible à la <b>casse</b>.</param><param name="root">🌳 Nœud courant dont les enfants sont analysés à cette itération.</param><param name="logIt">📝 Si <c>true</c>, journalise les trouvailles et l'échec final via <see cref="CustomLogger"/>.</param><param name="isRoot">🔑 Indique si cet appel est le <b>niveau racine</b> de la récursion, pour conditionner le log d'échec.</param><returns>🔗 Le <see cref="Node"/> trouvé, ou <c>null</c> si aucun enfant ne correspond.</returns>
	internal static Node GetNodeByName(string name, Node root, bool logIt, bool isRoot = true)
	{
		foreach (Node child in root.GetChildren(true))
		{
			if (child.Name == name)
			{
				if (logIt) CustomLogger.Log("[[rainbow]TypeSearcher[/rainbow]]", IconPaths.PARAMMETHODBINDING, $" [wave]{child.Name}[/wave] : trouvé");
				return child;
			}

			Node found = GetNodeByName(name, child, logIt, isRoot: false);
			if (found != null) return found;
		}

		// Log uniquement à la fin de la recherche complète (niveau racine)
		if (isRoot && logIt) CustomLogger.Log("[[pulse]TypeSearcher[/pulse]]", IconPaths.PARAMMETHODBINDING, $" Noeud introuvable");
		return null;
	}

	/// <summary> 👶Chercheur de Nœuds enfants<br/><br/>Parcourt uniquement les <b>enfants directs</b> d’un nœud parent et retourne ceux correspondant au type <c>T</c>.<br/><br/><b>📋</b> Contrairement à <see cref="GetAllNodeOfType{T}"/>, cette méthode n’effectue <b>aucune récursion</b>.<br/><br/><b>🎓</b> Utile pour filtrer rapidement des composants enfants sans impact sur les performances.</summary> <typeparam name="T">Type de nœud recherché (héritant de <see cref="Node"/>).</typeparam> <param name="NodeParent">Nœud parent dont les enfants directs seront analysés.</param> <returns>Un Tableau contenant uniquement les enfants correspondant au type <c>T</c>.</returns>
	public static Array<T> GetChildrenOfType<[MustBeVariant] T>(Node NodeParent) where T : Node
	{
		// Liste temporaire pour stocker tous les enfants du type recherché
		Array<T> childrenFound = [];

		// Parcours de tous les nœuds enfants
		for (int i = 0; i < NodeParent.GetChildren(true).Count; i++)
		{
			// Vérification si l'enfant est du Type recherché
			if (NodeParent.GetChild(i) is T foundChild)
			{
				// Ajout de l'enfant trouvé à la liste
				childrenFound.Add(foundChild);
			}
		}

		// Conversion de la liste en tableau et retour du résultat
		return childrenFound;
	}

	public static Array<T> GetAllChildrenOfType<[MustBeVariant] T>(Node NodeParent) where T : Node
	{
		Array<T> childrenFound = [];

		foreach (Node child in NodeParent.GetChildren(true))
		{
			if (child is T typedChild)
				childrenFound.Add(typedChild);

			// Récursion sur les enfants de l'enfant
			Array<T> deepChildren = GetAllChildrenOfType<T>(child);
			foreach (T deepChild in deepChildren)
				childrenFound.Add(deepChild);
		}

		return childrenFound;
	}
	public static T GetFirstNodeOfType<[MustBeVariant] T>(bool logIt = false, bool isRoot = true) where T : Node => GetFirstNodeOfType<T>(GetRoot(), logIt, isRoot);
	private static T GetFirstNodeOfType<[MustBeVariant] T>(Node root, bool logIt = false, bool isRoot = true) where T : Node
	{
		for (int i = 0; i < root.GetChildCount(true); i++)
		{
			Node child = root.GetChild(i, true);

			if (child is T typedChild)
			{
				if (logIt) CustomLogger.Log("[[rainbow]TypeSearcher[/rainbow]]", IconPaths.PARAMMETHODBINDING, $" [wave]{typedChild.Name}[/wave] : trouvé");
				return typedChild;
			}

			T found = GetFirstNodeOfType<T>(child, logIt, isRoot: false);
			if (found != null) return found;
		}

		if (isRoot && logIt) CustomLogger.Log("[[pulse]TypeSearcher[/pulse]]", IconPaths.PARAMMETHODBINDING, $" Noeud introuvable");
		return null;
	}

	public static SceneTree GetTree()
	{
		if (Engine.GetMainLoop() is SceneTree tree)
		{
			return tree;
		}

		GD.PrintErr("[[pulse]TypeSearcher[/pulse]] Arbre de Scène introuvable.");
		return null;
	}
	public static Node GetRoot() => GetTree().Root;

}