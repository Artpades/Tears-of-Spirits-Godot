using Godot;
using System;
namespace GameplayToolkit.Cameras
{
#if TOOLS
	[GlobalClass, Tool, Icon(IconPaths.CAMERAMANAGER)]
#endif
	public partial class CameraManager : Node
	{
		private static CameraManager _instance;
		public static CameraManager Instance => ManagerBuilder.MakeGetter(ref _instance);
		public override void _EnterTree()
		{
			ManagerBuilder.InitInstance(ref _instance, this);
		}
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			
		}

		public void Switch2DCamera(VirtualCamera2D newCurrentCamera)
		{
			
		
			
		}
	
	
		public void Switch3DCamera(VirtualCamera3D newCurrentCamera)
		{
			
		
			
		}
	}

}