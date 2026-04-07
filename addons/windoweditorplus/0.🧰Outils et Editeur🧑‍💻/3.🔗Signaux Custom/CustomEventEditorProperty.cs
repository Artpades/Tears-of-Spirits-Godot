#if TOOLS
using Godot;
using Godot.Collections;
namespace GameplayToolkit.Editor
{
	[Tool]
	public partial class CustomEventEditorProperty : EditorProperty
	{
		private VBoxContainer _container;
		private Button _addButton;
		private CustomEvent _currentEvent;

		public override void _Ready()
		{
			_container = new VBoxContainer();
			AddChild(_container);
			SetBottomEditor(_container);

			_addButton = new Button { Text = "➕ Ajouter un récepteur" };
			_addButton.Pressed += OnAddPressed;
			_container.AddChild(_addButton);
		}

		public override void _UpdateProperty()
		{
			_currentEvent = GetEditedObject().Get(GetEditedProperty()).As<CustomEvent>();
			Refresh();
		}

		private void Refresh()
		{
			// Supprimer les anciennes lignes (sauf le bouton Add)
			foreach (Node child in _container.GetChildren())
			{
				if (child != _addButton)
					child.QueueFree();
			}

			if (_currentEvent == null) return;

			for (int i = 0; i < _currentEvent.Targets.Count; i++)
			{
				int index = i; // capture pour la lambda
				var row = new HBoxContainer();

				var pathEdit = new LineEdit
				{
					Text = _currentEvent.Targets[i],
					SizeFlagsHorizontal = SizeFlags.ExpandFill,
					PlaceholderText = "NodeName..."
				};
				pathEdit.TextChanged += text =>
				{
					_currentEvent.Targets[index] = text;
					EmitChanged(GetEditedProperty(), _currentEvent);
				};

				var methodEdit = new LineEdit
				{
					Text = _currentEvent.Methods[i],
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
					PlaceholderText = "MethodName..."
				};
				methodEdit.TextChanged += text =>
				{
					_currentEvent.Methods[index] = text;
					EmitChanged(GetEditedProperty(), _currentEvent);
				};

				var removeBtn = new Button { Text = "❌" };
				removeBtn.Pressed += () =>
				{
					_currentEvent.Targets.RemoveAt(index);
					_currentEvent.Methods.RemoveAt(index);
					EmitChanged(GetEditedProperty(), _currentEvent);
					Refresh();
				};

				row.AddChild(pathEdit);
				row.AddChild(methodEdit);
				row.AddChild(removeBtn);
				_container.AddChild(row);
				_container.MoveChild(row, index); // avant le bouton Add
			}
		}

		private void OnAddPressed()
		{
			if (_currentEvent == null)
			{
				_currentEvent = new CustomEvent();
				EmitChanged(GetEditedProperty(), _currentEvent);
			}

			_currentEvent.Targets.Add("");
			_currentEvent.Methods.Add("");
			EmitChanged(GetEditedProperty(), _currentEvent);
			Refresh();
		}
	}
}
#endif