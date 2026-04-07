#if TOOLS
using GameplayToolkit.CharacterAndAlgorithm;
using GameplayToolkit.InputAndControls;
using Godot;
using System.Collections.Generic;
using System;

namespace GameplayToolkit.ToolsAndEdit
{
	[GlobalClass, Tool, Icon(IconPaths.GIZMOS)]
	public partial class GizmosSprite3D : Node3D
	{
		private float _distanceLimit = 0.5f;
		[Export]
		public float DistanceLimit
		{
			get => _distanceLimit;
			set
			{
				if (_distanceLimit != value)
				{
					if (!Engine.IsEditorHint()) return;
					_distanceLimit = value;
					visual.Scale = new(15 * DistanceLimit, 15 * DistanceLimit, 15 * DistanceLimit);

				}
			}
		}
		[Export] public Color GizmosColor = new(1, 1, 1, 1);

		private bool _handle = false;
		private Camera3D _camera => EditorInterface.Singleton.GetEditorViewport3D().GetCamera3D();
		private Sprite3D visual = new();
		private bool _wasMousePressed = false;

		private Node _nodeTarget => GetParent();
		[Export]
		public Node TargetNodeSelection
		{
			get => _nodeTarget;
			set
			{
				if (!Engine.IsEditorHint()) return;
				UpdateVisual(_nodeTarget);
			}
		}
		private static readonly Dictionary<Type, string> _iconPaths = new()
		{
			{typeof(PostIt), IconPaths.POSTIT},
			{ typeof(PlayerInput), IconPaths.INPUTMANAGER },
			{ typeof(StateMachine), IconPaths.HFSM },
		};
		internal void UpdateVisual(Node nodeToAnalyse)
		{
			if (nodeToAnalyse is null) return;

			if (_iconPaths.TryGetValue(nodeToAnalyse.GetType(), out string pathToUse))
			{
				visual.Texture = ResourceLoader.Load<Texture2D>(pathToUse);
			}
			else
			{
				CustomLogger.Log(Name, IconPaths.GIZMOS, "[color=yellow]Classe non prise en compte ![/color]");
				visual.Texture = ResourceLoader.Load<Texture2D>(IconPaths.GIZMOS);
			}
		}
		public override void _EnterTree()
		{
			if (!Engine.IsEditorHint())
			{
				QueueFree();

				return;
			}
			else
				EditorInterface.Singleton.GetSelection().SelectionChanged += OnSelectionChanged;

		}
		private void OnSelectionChanged()
		{
			if (EditorInterface.Singleton.GetSelection().GetSelectedNodes().Contains(TargetNodeSelection))
			{
				GizmosColor = new(1, 1, 1, 1);
				visual.Modulate = GizmosColor;

			}
			else
			{
				GizmosColor = new(0.5f, 0.5f, 0.5f, 1);
				visual.Modulate = GizmosColor;

			}

		}
		public override void _Ready()
		{
			if (!Engine.IsEditorHint()) return;

			visual.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
			visual.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;
			visual.Scale = new(3, 3, 3);
			visual.Texture = ResourceLoader.Load<Texture2D>(IconPaths.POSTIT);
			TargetNodeSelection = _nodeTarget;
			if (!visual.IsInsideTree())
				AddChild(visual);
		}

		public override void _Process(double delta)
		{
			if (!Engine.IsEditorHint()) return;

			if (_camera == null) return;

			Vector2 mousePos = EditorInterface.Singleton.GetEditorViewport3D().GetMousePosition();
			bool isHovered = IsMouseInSquare(mousePos);

			bool mousePressed = Input.IsMouseButtonPressed(MouseButton.Left);
			bool justReleased = !mousePressed && _wasMousePressed;
			_wasMousePressed = mousePressed;

			if (isHovered && justReleased && TargetNodeSelection != null)
			{
				if (!EditorInterface.Singleton.GetSelection().GetSelectedNodes().Contains(TargetNodeSelection))
					Redirect();
			}
		}

		private bool IsMouseInSquare(Vector2 mousePos)
		{
			Vector2 screenPos = _camera.UnprojectPosition(GlobalPosition);
			Vector3 offsetWorld = GlobalPosition + _camera.GlobalBasis.X.Normalized() * DistanceLimit;
			Vector2 offsetScreen = _camera.UnprojectPosition(offsetWorld);
			float halfSize = screenPos.DistanceTo(offsetScreen);

			return Mathf.Abs(mousePos.X - screenPos.X) <= halfSize && Mathf.Abs(mousePos.Y - screenPos.Y) <= halfSize;
		}

		private Vector3 ProjectMouseOnCameraPlane(Vector2 mousePos)
		{
			Vector3 planeNormal = -_camera.GlobalBasis.Z.Normalized();
			Vector3 planePoint = GlobalPosition;
			Vector3 rayOrigin = _camera.ProjectRayOrigin(mousePos);
			Vector3 rayDir = _camera.ProjectRayNormal(mousePos);

			float denom = rayDir.Dot(planeNormal);
			if (Mathf.Abs(denom) < 0.001f) return GlobalPosition;
			float t = (planePoint - rayOrigin).Dot(planeNormal) / denom;
			if (t < 0) return GlobalPosition;
			return rayOrigin + rayDir * t;
		}

		private void Redirect()
		{
			EditorInterface.Singleton.GetSelection().Clear();
			EditorInterface.Singleton.GetSelection().AddNode(TargetNodeSelection);
		}

	}
}
#endif