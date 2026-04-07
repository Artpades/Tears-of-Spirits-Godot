using Godot;

namespace GameplayToolkit.UIUX
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.HUD)]
#endif
	public partial class HUD : Sprite3D
	{
		#region 🔧 Champs Privés

		private Vector2? lastEventPos2D = null;

		private float lastEventTime = -1.0f;

		#endregion



		#region 🎛️ Exports

		[Export] private SubViewport nodeViewport;

		[Export] private Area3D nodeArea;


		#endregion



		#region 🚀 Cycle de Vie Godot

		public override void _Ready()
		{
			// Met à jour la gestion souris (si activée dans un script parent ou propriété externe).
			UpdateMouseInteraction();

			// Désactive _Process si le Sprite3D n'utilise pas le billboard.
			if (Billboard == BaseMaterial3D.BillboardModeEnum.Disabled)
			{
				SetProcess(false);
			}
		}


		public override void _Process(double delta)
		{
			RotateAreaToBillboard();
		}

		#endregion



		#region 🖱️ Gestion des Entrées

		public override void _Input(InputEvent @event)
		{
			// Si l'interaction souris n'est pas autorisée, on ignore l'événement.
			if (!InteractWithMouse) return;

			// Ignore mouse/touch here because they are handled via 3D physics picking.
			if (@event is InputEventMouseButton or InputEventMouseMotion or InputEventScreenDrag or InputEventScreenTouch)
			{
				return;
			}

			// Pousse l'événement d'entrée dans le SubViewport pour interaction UI.
			nodeViewport.PushInput(@event);
		}

		#endregion



		#region 🎯 Orientation Billboard

		private void RotateAreaToBillboard()
		{
			var billboardMode = Billboard;

			// Si le billboard n'est pas actif → rien à faire.
			if (billboardMode == BaseMaterial3D.BillboardModeEnum.Disabled)
				return;

			// Récupère la caméra active du viewport.
			var camera = GetViewport().GetCamera3D();
			if (camera == null) return;

			// Calcule une direction de regard alignée avec la caméra.
			var look = camera.ToGlobal(new Vector3(0, 0, -100)) - camera.GlobalTransform.Origin;
			look = nodeArea.Position + look;

			// Billboard Y : verrouille l’axe Y (peut donner de mauvais résultats si la caméra est inclinée).
			if (billboardMode == BaseMaterial3D.BillboardModeEnum.FixedY)
			{
				look = new Vector3(look.X, 0, look.Z);
			}

			// Oriente la zone vers la direction calculée.
			nodeArea.LookAt(look, Vector3.Up);

			// Compense l'inclinaison Z de la caméra pour éviter une rotation non désirée.
			nodeArea.RotateObjectLocal(Vector3.Back, camera.Rotation.Z);
		}

		#endregion



		#region 🎛️ Propriété : InteractWithMouse

		[Export] public bool InteractWithMouse;

		#endregion



		#region 🖱️ États internes d’interaction souris

		private bool isMouseInside = false;

		#endregion



		#region 🔄 Mise à jour de l’interaction souris

		private void UpdateMouseInteraction()
		{
			if (nodeArea == null) return;

			if (InteractWithMouse)
			{
				// Abonnements aux événements similaires à OnMouseEntered  / Exited / InputEvent
				nodeArea.MouseEntered += OnMouseEnteredArea;
				nodeArea.MouseExited += OnMouseExitedArea;
				nodeArea.InputEvent += OnMouseInputEvent;

				// Active la capacité de recevoir des raycasts pour les interactions
				nodeArea.InputRayPickable = true;
			}
			else
			{
				// Désactive le ray picking
				nodeArea.InputRayPickable = false;

				// Réinitialise l'état interne
				isMouseInside = false;
			}
		}

		#endregion



		#region 📌 Callbacks Area3D

		private void OnMouseEnteredArea()
		{
			isMouseInside = true;
		}

		private void OnMouseExitedArea()
		{
			isMouseInside = false;
		}

		#endregion



		#region 🎚️ Traitement avancé des événements Input

		private void OnMouseInputEvent(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, long shapeIdx)
		{
			// Taille effective du Sprite3D (texture × pixel size)
			Vector2 spriteSize = Vector2.Zero;

			if (Texture != null)
			{
				Vector2 textureSize = Texture.GetSize();
				spriteSize = textureSize * PixelSize;
			}

			// Position 3D brute de l'événement (dans l'espace du monde)
			Vector3 eventPos3D = eventPosition;

			// Récupère temps actuel en secondes
			float now = Time.GetTicksMsec() / 1000.0f;

			// Convertit la position de l'événement dans l'espace local du Sprite3D
			eventPos3D = GlobalTransform.AffineInverse() * eventPos3D;

			// Position 2D convertie
			Vector2 eventPos2D = Vector2.Zero;

			if (isMouseInside)
			{
				// 3D → 2D : X = X, Y inversé
				eventPos2D = new Vector2(eventPos3D.X, -eventPos3D.Y);

				// (-sprite/2 → +sprite/2) → (-0.5 → 0.5)
				eventPos2D.X /= spriteSize.X;
				eventPos2D.Y /= spriteSize.Y;

				// (-0.5 → 0.5) → (0 → 1)
				eventPos2D += new Vector2(0.5f, 0.5f);

				// (0 → 1) → (0 → viewport size)
				eventPos2D *= nodeViewport.Size;
			}
			else if (lastEventPos2D != null)
			{
				// Quand la souris quitte l'Area3D, on conserve la dernière position UI connue
				eventPos2D = lastEventPos2D.Value;
			}

			#region 🔧 Réaffectation position event

			if (@event is InputEventMouse mouseEvent)
			{
				mouseEvent.Position = eventPos2D;
				mouseEvent.GlobalPosition = eventPos2D;
			}
			else if (@event is InputEventScreenTouch touchEvent)
			{
				touchEvent.Position = eventPos2D;
			}
			else if (@event is InputEventScreenDrag dragEventPos)
			{
				dragEventPos.Position = eventPos2D;
			}

			#endregion


			#region 📐 Calcul movement relatif / vitesse

			if (@event is InputEventMouseMotion motionEvent)
			{
				if (lastEventPos2D == null)
				{
					motionEvent.Relative = Vector2.Zero;
				}
				else
				{
					motionEvent.Relative = eventPos2D - lastEventPos2D.Value;
					motionEvent.Velocity = motionEvent.Relative / (now - lastEventTime);
				}
			}
			else if (@event is InputEventScreenDrag dragEvent)
			{
				if (lastEventPos2D == null)
				{
					dragEvent.Relative = Vector2.Zero;
				}
				else
				{
					dragEvent.Relative = eventPos2D - lastEventPos2D.Value;
					dragEvent.Velocity = dragEvent.Relative / (now - lastEventTime);
				}
			}

			#endregion


			// Mise à jour des dernières valeurs
			lastEventPos2D = eventPos2D;
			lastEventTime = now;

			// Envoi final de l'événement transformé au SubViewport
			nodeViewport.PushInput(@event);
		}

		#endregion
	}

}
