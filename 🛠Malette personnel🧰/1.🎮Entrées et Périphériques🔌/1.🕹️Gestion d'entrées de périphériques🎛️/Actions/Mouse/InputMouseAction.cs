using Godot;
using System;
namespace GameplayToolkit.InputAndControls
{
    /// <summary> <b>🖱️ Action d'Entrée Souris</b><br/><br/><b>📋</b> Gère les entrées de la souris : position absolue et mouvement relatif.<br/><br/><b>🎓</b> Utilisée pour capturer la position et le delta de mouvement de la souris.</summary>
#if TOOLS
    [GlobalClass, Tool, Icon(IconPaths.INPUTACTIONMOUSE)]
#endif
    public partial class InputMouseAction : InputAction
    {
        #region Signaux

        /// <summary><b>📍 Position Émise</b><br/><br/>📋 Signal émis <c>chaque frame</c> avec la position actuelle de la souris dans le viewport.<br/><br/>🎓 Utilisé pour le suivi continu de la position du curseur.</summary>
        [Signal] public delegate void PositionEmittedEventHandler(Vector2 position);

        /// <summary><b>🏃 Mouvement Émis</b><br/><br/>📋 Signal émis <c>chaque frame</c> avec le delta de mouvement accumulé (mouvement relatif).<br/><br/>🎓 Utilisé pour les systèmes nécessitant la vitesse/direction de déplacement.</summary>
        [Signal] public delegate void MotionEmittedEventHandler(Vector2 motion);

        /// <summary><b>✏️ Pression Stylet</b><br/><br/>📋 Signal émis lorsque la pression du stylet change.<br/><br/>🎓 Valeur entre 0.0 (pas de pression) et 1.0 (pression maximale). Retourne 1.0 pour une souris classique.</summary>
        [Signal] public delegate void PressureChangedEventHandler(float pressure);
        #endregion

        #region Configuration Exportée


        /// <summary><b>🎚️ Sensibilité du Mouvement</b><br/><br/>📋 Multiplicateur appliqué au mouvement relatif de la souris (entre <c>0.1</c> et <c>5.0</c>).<br/><br/>🎓 Augmenter pour une souris plus réactive, diminuer pour plus de précision.</summary>
        [ExportGroup("⚙️ Paramètres")]
        [Export(PropertyHint.Range, "0.1,5.0,0.1")]
        public float MotionSensitivity { get; set; } = 1.0f;


        /// <summary><b>🚧 Seuil de Mouvement</b><br/><br/>📋 Magnitude minimale du mouvement pour déclencher <see cref="MotionChanged"/> (entre <c>0</c> et <c>10</c>).<br/><br/>🎓 Permet de filtrer les tremblements ou mouvements involontaires.</summary>
        [Export(PropertyHint.Range, "0,10,0.5")]
        public float MotionThreshold { get; set; } = 0.5f;


        /// <summary><b>📐 Normaliser la Position</b><br/><br/>📋 Si <c>true</c>, la position est normalisée entre <c>0</c> et <c>1</c> par rapport à la taille du viewport.<br/><br/>🎓 Utile pour des systèmes indépendants de la résolution d'écran.</summary>
        [Export]
        public bool NormalizePosition { get; set; } = false;
        #endregion

        #region État Interne

        /// <summary><b>📝 Position Précédente</b><br/><br/>📋 Stocke la position de la souris de la frame précédente pour détecter les changements.<br/><br/>🎓 Utilisée pour l'émission conditionnelle du signal <see cref="PositionChanged"/>.</summary>
        private Vector2 _previousPosition = Vector2.Zero;


        /// <summary><b>📦 Mouvement Accumulé</b><br/><br/>📋 Accumule les mouvements relatifs entre deux appels de <see cref="ProcessInputDetection"/>.<br/><br/>🎓 Permet de capturer tout le mouvement même si plusieurs événements arrivent par frame.</summary>
        private Vector2 _accumulatedMotion = Vector2.Zero;


        /// <summary><b>✏️ Pression Actuelle</b><br/><br/>📋 Stocke la pression actuelle du stylet.<br/><br/>🎓 Valeur entre 0.0 et 1.0.</summary>
        private float _currentPressure = 0f;


        /// <summary><b>✏️ Pression Précédente</b><br/><br/>📋 Stocke la pression de la frame précédente pour détecter les changements.<br/><br/>🎓 Utilisée pour l'émission conditionnelle du signal <see cref="PressureChanged"/>.</summary>
        private float _previousPressure = 0f;


        /// <summary><b>✅ Viewport Disponible</b><br/><br/>📋 Indique si un <see cref="Viewport"/> valide a été assigné via <see cref="SetViewport"/>.<br/><br/>🎓 Protège contre les appels sans viewport configuré.</summary>
        private bool _hasViewport = false;


        /// <summary><b>🖼️ Référence au Viewport</b><br/><br/>📋 Le <see cref="Viewport"/> utilisé pour obtenir la position de la souris et la taille de l'écran.<br/><br/>🎓 Doit être configuré via <see cref="SetViewport"/> avant utilisation.</summary>
        private Viewport _viewport;
        #endregion
        public override void _Ready()
        {
            SetViewport(GetViewport());
        }

        public override void _Process(double delta)
        {
            // Vérifier qu'un viewport est configuré
            if (!_hasViewport) return;

            // Récupérer les valeurs actuelles
            Vector2 currentPosition = GetCurrentPosition();
            Vector2 currentMotion = _accumulatedMotion * MotionSensitivity;

            // Émission continue de la position (chaque frame)
            EmitSignal(SignalName.PositionEmitted, currentPosition);

            // Émission continue du mouvement (chaque frame)
            EmitSignal(SignalName.MotionEmitted, currentMotion);


            // Émission si la pression du stylet a changé
            if (!Mathf.IsEqualApprox(_currentPressure, _previousPressure))
            {
                EmitSignal(SignalName.PressureChanged, _currentPressure);
                _previousPressure = _currentPressure;
            }

            // Réinitialiser le mouvement accumulé pour la prochaine frame
            _accumulatedMotion = Vector2.Zero;
        }

        public override void _Input(InputEvent @event)
        {
            if (Engine.IsEditorHint() && ProcessMode == ProcessModeEnum.Disabled) return;

            // Transmettre les événements de mouvement souris aux InputMouseAction
            if (@event is InputEventMouseMotion motionEvent)
            {
                AccumulateMotion(motionEvent);
            }
        }
        /// <summary><b>🖼️ Définir le Viewport</b><br/><br/>📋 Configure le <see cref="Viewport"/> utilisé pour lire la position de la souris.<br/><br/>🎓 Doit être appelée par le <see cref="InputManagerComplex"/> lors de l'initialisation.</summary> <param name="viewport">Le <see cref="Viewport"/> à utiliser, ou <c>null</c> pour désactiver.</param>
        public void SetViewport(Viewport viewport)
        {
            _viewport = viewport;
            _hasViewport = viewport != null;
            // Initialiser la position précédente pour éviter un faux changement à la première frame
            if (_hasViewport)
            {
                _previousPosition = GetCurrentPosition();
            }
        }
        /// <summary><b>➕ Accumuler le Mouvement (Événement)</b><br/><br/>📋 Ajoute le mouvement relatif d'un <see cref="InputEventMouseMotion"/> au total accumulé.<br/><br/>🎓 Appelée par le <see cref="InputManagerComplex"/> lors de la réception d'événements souris.</summary> <param name="motionEvent">L'événement de mouvement souris à accumuler.</param>
        public void AccumulateMotion(InputEventMouseMotion motionEvent)
        {
            if (motionEvent == null) return;
            _accumulatedMotion += motionEvent.Relative;
            _currentPressure = motionEvent.Pressure;
        }


        /// <summary><b>➕ Accumuler le Mouvement (Vecteur)</b><br/><br/>📋 Ajoute un vecteur de mouvement relatif au total accumulé.<br/><br/>🎓 Surcharge pour l'ajout direct sans événement.</summary> <param name="relativeMotion">Le vecteur de mouvement à accumuler.</param>
        public void AccumulateMotion(Vector2 relativeMotion)
        {
            _accumulatedMotion += relativeMotion;
        }


        #region Méthodes Utilitaires

        /// <summary><b>📍 Obtenir la Position Actuelle (Interne)</b><br/><br/>📋 Calcule la position de la souris dans le viewport, avec normalisation optionnelle.<br/><br/>🎓 Méthode privée utilisée par <see cref="ProcessInputDetection"/> et <see cref="GetPosition"/>.</summary>
        private Vector2 GetCurrentPosition()
        {
            if (!_hasViewport) return Vector2.Zero;

            // Lire la position brute de la souris dans le viewport
            Vector2 mousePos = _viewport.GetMousePosition();

            // Normaliser si demandé (convertir en coordonnées 0-1)
            if (NormalizePosition)
            {
                Vector2 viewportSize = _viewport.GetVisibleRect().Size;
                if (viewportSize.X > 0 && viewportSize.Y > 0)
                {
                    mousePos = new Vector2(
                        mousePos.X / viewportSize.X,
                        mousePos.Y / viewportSize.Y
                    );
                }
            }

            return mousePos;
        }


        /// <summary><b>📍 Obtenir la Position</b><br/><br/>📋 Retourne la position actuelle de la souris dans le viewport.<br/><br/>🎓 Méthode publique pour lecture ponctuelle de la position.</summary>
        public Vector2 GetPosition()
        {
            return GetCurrentPosition();
        }


        /// <summary><b>🏃 Obtenir le Mouvement Accumulé</b><br/><br/>📋 Retourne le mouvement relatif accumulé depuis le dernier <see cref="ProcessInputDetection"/>, multiplié par la sensibilité.<br/><br/>🎓 Méthode publique pour lecture ponctuelle du mouvement.</summary>
        public Vector2 GetAccumulatedMotion()
        {
            return _accumulatedMotion * MotionSensitivity;
        }


        /// <summary><b>✏️ Obtenir la Pression</b><br/><br/>📋 Retourne la pression actuelle du stylet.<br/><br/>🎓 Valeur entre 0.0 et 1.0. Retourne 1.0 pour une souris classique sans support de pression.</summary>
        public float GetPressure()
        {
            return _currentPressure;
        }
        #endregion
    }

}