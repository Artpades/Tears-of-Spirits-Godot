using Godot;
using System;

namespace GameplayToolkit
{
	public static partial class ManagerBuilder
	{
		/// <summary>
		/// Retourne l'instance singleton existante du type T dans la scène,
		/// ou en crée une nouvelle si elle n'existe pas.
		/// </summary>
		public static T MakeGetter<[MustBeVariant] T>(ref T instance) where T : Node, new()
		{
			// Si l'instance existe déjà, on la retourne directement
			if (instance != null) return instance;

			// On tente de la trouver dans la scène
			instance = TypeSearcher.GetFirstNodeOfType<T>();

			// Si elle n'existe toujours pas, on en crée une nouvelle
			if (instance == null)
			{
				T singletonObject = new();
				instance = singletonObject;

				singletonObject.Name = typeof(T).ToString() + " (Singleton)";
				GD.Print($"Un Singleton de {typeof(T).Name} a été créé !");

				// Ajout différé à la racine pour éviter les problèmes de cycle de vie
				TypeSearcher.GetRoot().CallDeferred(Node.MethodName.AddChild, instance);
			}

			return instance;
		}

		/// <summary>
		/// Initialise le singleton : détruit les doublons ou s'enregistre comme instance unique.
		/// À appeler dans _EnterTree() ou _Ready() du manager.
		/// </summary>
		public static void InitInstance<[MustBeVariant] T>(ref T instance, T caller) where T : Node
		{
			// Un doublon existe déjà → on détruit l'appelant
			if (instance != null && !instance.Equals(caller))
			{
				caller.QueueFree();
			}
			else
			{
				// On s'enregistre comme instance canonique
				instance = caller;
			}
		}
	}

}
