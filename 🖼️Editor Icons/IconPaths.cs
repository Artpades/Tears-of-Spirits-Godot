using Godot;
using System;

namespace GameplayToolkit;
/// <summary> <b>🖼️Atlas d'Icônes pour Editeur</b><br/><br/> 📋 Répertorie des chemins pour récupérer des textures d'icônes.</summary>
public static partial class IconPaths
{
	#region 🧰
	public const string WINDOW = "res://🖼️Editor Icons/0.🧰🧑‍💻/WindowEditor icon.png";

	public const string MANAGERS = "res://🖼️Editor Icons/0.🧰🧑‍💻/ManagerBase icon.png";

	public const string METHODBINDING = "res://🖼️Editor Icons/0.🧰🧑‍💻/MethodAction icon.png";
	public const string PARAMMETHODBINDING = "res://🖼️Editor Icons/0.🧰🧑‍💻/ParamsMethodAction icon.png";

	public const string GIZMOS = "res://🖼️Editor Icons/0.🧰🧑‍💻/Pix. Gizmos Icon.png";

	public const string SIGNALEMITTER = "res://🖼️Editor Icons/0.🧰🧑‍💻/SignalEmitter icon.png";
	public const string SIGNALRECEIVER = "res://🖼️Editor Icons/0.🧰🧑‍💻/SignalReceiver icon.png";


	public const string TASKMANAGER = "res://🖼️Editor Icons/0.🧰🧑‍💻/TaskManagement icon.png";
	public const string POSTIT = "res://🖼️Editor Icons/0.🧰🧑‍💻/Post-It icon.png";
	public const string THUMBTACK = "res://🖼️Editor Icons/0.🧰🧑‍💻/Thumbtack icon.png";
	public const string PADLOCK = "res://🖼️Editor Icons/0.🧰🧑‍💻/Padlock icon.png";
	#endregion

	#region 🎮
	public const string INPUTMANAGER = "res://🖼️Editor Icons/1.🎮/InputManager Icon.png";
	public const string INPUTCONTEXT = "res://🖼️Editor Icons/1.🎮/InputContexte Icon.png";
	public const string INPUTBUTTONBUNDLE = "res://🖼️Editor Icons/1.🎮/ButtonAssigned Icon.png";
	public const string INPUTACTION = "res://🖼️Editor Icons/1.🎮/InputAction Icon.png";
	public const string INPUTACTIONBUTTON = "res://🖼️Editor Icons/1.🎮/InputButtonAction Icon.png";
	public const string INPUTACTIONAXIS = "res://🖼️Editor Icons/1.🎮/InputAxisAction Icon.png";
	public const string INPUTACTIONCIRCLE = "res://🖼️Editor Icons/1.🎮/InputVector2Action Icon.png";
	public const string INPUTACTIONSPHERE = "res://🖼️Editor Icons/1.🎮/InputVector3Action Icon-export.png";
	public const string INPUTACTIONMOUSE = "res://🖼️Editor Icons/1.🎮/InputMouseAction Icon.png";
	public const string INPUTACTIONSCREEN = "res://🖼️Editor Icons/1.🎮/InputScreenSphere Icon.png";
	public const string INPUTACTIONMIDI = "res://🖼️Editor Icons/1.🎮/InputMIDIAction Icon.png";
	public const string INPUTLEVER = "res://🖼️Editor Icons/1.🎮/InputLever Icon.png";
	public const string TRIGGER = "res://🖼️Editor Icons/1.🎮/Trigger Icon.png";
	public const string DPAD = "res://🖼️Editor Icons/1.🎮/DPad Icon.png";
	public const string JOYPADMOTION = "res://🖼️Editor Icons/1.🎮/JoyPadMotion Icon.png";
	public const string INPUTJOYSPHERE = "res://🖼️Editor Icons/1.🎮/InputJoySphere Icon.png";
	public const string INPUTBINDING = "res://🖼️Editor Icons/1.🎮/InputMapBinding Icon.png";

	public const string CURSORMANAGER = "res://🖼️Editor Icons/1.🎮/CursorManager Icon.png";
	public const string PROFILECURSOR = "res://🖼️Editor Icons/1.🎮/CursorProfile Icon.png";


	#endregion

	#region 🎥

	public const string CAMERAMANAGER = "res://🖼️Editor Icons/2.🎥/CameraManager Icon.png";
	public const string VIRTUALCAM = "res://🖼️Editor Icons/2.🎥/VirtualCamera Icon.png";

	public const string ORBITALCAMERA = "res://🖼️Editor Icons/2.🎥/OrbitalCamera Icon.png";

	public const string CAMEFFECT = "res://🖼️Editor Icons/2.🎥/CameraEffect Icon.png";
	public const string CAMSHAKEEFFECT = "res://🖼️Editor Icons/2.🎥/Camera ShakeEffect Icon.png";
	public const string CAMZOOMEFFECT = "res://🖼️Editor Icons/2.🎥/Camera ZoomEffect Icon.png";
	public const string CAMLOOKATEFFECT = "res://🖼️Editor Icons/2.🎥/Camera LookAtEffect Icon.png";
	public const string CAMFOLLOWEFFECT = "res://🖼️Editor Icons/2.🎥/Camera FollowEffect Icon.png";
	#endregion

	#region 🧍🧠
	public const string CHARACTER = "res://🖼️Editor Icons/3.🧍🧠/Character Icon.png";
	public const string PLAYER = "res://🖼️Editor Icons/3.🧍🧠/Player Icon.png";
	public const string AUTOMATE = "res://🖼️Editor Icons/3.🧍🧠/Automate Icon.png";
	public const string CHARACTERREGISTER = "res://🖼️Editor Icons/3.🧍🧠/CharacterRegister Icon.png";

	public const string PROJECTILE = "res://🖼️Editor Icons/3.🧍🧠/Projectile Icon.png";

	public const string HFSM = "res://🖼️Editor Icons/3.🧍🧠/StateMachine Icon.png";
	public const string STATE = "res://🖼️Editor Icons/3.🧍🧠/State Icon.png";
	#endregion

	#region 🌀

	public const string CHARACTERMOVEMENT = "res://🖼️Editor Icons/4.🌀/CharacterMovement.png";
	public const string RIGIDBODYMOVEMENT = "res://🖼️Editor Icons/4.🌀/RigidBodyMovement.png";

	public const string RECOIL3D = "res://🖼️Editor Icons/4.🌀/Recoil3D Icon.png";
	public const string RECOILRIGIDBODY3D = "res://🖼️Editor Icons/4.🌀/RecoilPhysics3D Icon.png";
	public const string RECOILCHARACTER3D = "res://🖼️Editor Icons/4.🌀/RecoilCharacter3D Icon.png";


	#endregion

	#region 💾💰
	public const string GAMEPOINT = "res://🖼️Editor Icons/5.💾💰/GamePoint Icon.png";

	public const string GAUGE = "res://🖼️Editor Icons/5.💾💰/Gauge Icon.png";

	public const string SAVERMANAGER = "res://🖼️Editor Icons/5.💾💰/SaveManager Icon.png";
	public const string FLOPPYDISK = "res://🖼️Editor Icons/5.💾💰/FloppyDisk Icon.png";

	public const string ENCRYPTIONKEY = "res://🖼️Editor Icons/5.💾💰/EncryptionKey Icon.png";
	#endregion

	#region 🧱⏱️
	public const string SPAWNER = "res://🖼️Editor Icons/6.🧱⏱️/Spawner Icon.png";
	public const string SPWANERENTRY = "res://🖼️Editor Icons/6.🧱⏱️/SpawnerEntry Icon.png";

	public const string ROTATINGCRANK = "res://🖼️Editor Icons/6.🧱⏱️/Gouvernaille Icon.png";

	public const string COLLECTABLE = "res://🖼️Editor Icons/6.🧱⏱️/Collectable Icon.png";

	public const string INVENTORY = "res://🖼️Editor Icons/6.🧱⏱️/Inventory Icon.png";
	public const string INVENTORYITEM = "res://🖼️Editor Icons/6.🧱⏱️/Item Inventory Icon.png";

	public const string SPRITESWAPPER = "res://🖼️Editor Icons/6.🧱⏱️/SpriteSwapper Icon.png";

	public const string TARGETER = "res://🖼️Editor Icons/6.🧱⏱️/Targeter Icon.png";

	public const string TIMEMANIPULATOR = "res://🖼️Editor Icons/6.🧱⏱️/TimeManipulator Icon.png";

	public const string STOPWATCH = "res://🖼️Editor Icons/6.🧱⏱️/StopWatch Icon.png";

	public const string METRONOME = "res://🖼️Editor Icons/6.🧱⏱️/Metronome Icon.png";

	public const string CLOCK = "res://🖼️Editor Icons/6.🧱⏱️/Clock Icon.png";
	#endregion

	#region ⚔️

	public const string POINTANDCLICK = "res://🖼️Editor Icons/7.⚔️/Point And Click Icon.png";
	public const string GRABBABLE = "res://🖼️Editor Icons/7.⚔️/Grabbale Icon.png";

	public const string CRAFTINGSYSTEM = "res://🖼️Editor Icons/7.⚔️/Crafting System Icon.png";

	public const string SKILL = "res://🖼️Editor Icons/7.⚔️/Skill Icon.png";
	public const string SKILLTREE = "res://🖼️Editor Icons/7.⚔️/SkillTree Icon.png";

	public const string ACTION = "res://🖼️Editor Icons/7.⚔️/ActionBase Icon.png";
	public const string ATTACK = "res://🖼️Editor Icons/7.⚔️/AttackBase Icon.png";
	public const string SHIELD = "res://🖼️Editor Icons/7.⚔️/ShieldBase Icon.png";
	public const string DODGE = "res://🖼️Editor Icons/7.⚔️/DodgeBase Icon.png";
	public const string PROJECTILEPOWER = "res://🖼️Editor Icons/7.⚔️/ProjectilePower Icon.png";
	public const string POWER = "res://🖼️Editor Icons/7.⚔️/Power Icon.png";
	#endregion

	#region 🧭
	public const string HUD = "res://🖼️Editor Icons/8.🧭/HUD Icon.png";

	public const string CANVA = "res://🖼️Editor Icons/8.🧭/Canva Icon.png";

	public const string DIALOGUE = "res://🖼️Editor Icons/8.🧭/Dialogue Icon.png";
	public const string DIALOGUESYSTEM = "res://🖼️Editor Icons/8.🧭/Dialogue System Icon.png";
	public const string EMPTYDIALOGUE = "res://🖼️Editor Icons/8.🧭/EmptyDialogue Icon.png";

	public const string SETTINGSPANEL = "res://🖼️Editor Icons/8.🧭/Settings Panel Icon.png";

	public const string TECHTREE = "res://🖼️Editor Icons/8.🧭/TechTree icon.png";
	public const string ITEMTECHTREE = "res://🖼️Editor Icons/8.🧭/Item TechTree icon.png";
	public const string NODETECHTREE = "res://🖼️Editor Icons/8.🧭/NodeTechTree icon.png";

	#endregion

	#region 🌍🌐
	public const string SCENEMANAGER = "res://🖼️Editor Icons/9.🌍🌐/Scene Manager Icon.png";
	#endregion

	#region ✨🌈🎵🔊
	public const string FANCYGUI = "res://🖼️Editor Icons/10. ✨🌈🎵🔊/Fancy UI Icon.png";

	public const string MUSICMANAGER = "res://🖼️Editor Icons/10. ✨🌈🎵🔊/MusicManager.png";

	#endregion

}
