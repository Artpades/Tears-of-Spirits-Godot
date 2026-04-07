#if TOOLS

using Godot;
using System;
namespace GameplayToolkit.ToolsAndEdit
{
	[Tool]
	public partial class GizmosLine3D : Gizmos3D
	{
		[Export] public Vector3 PointA = Vector3.Zero;
		[Export] public Vector3 PointB = Vector3.Right * 5;
		private ImmediateMesh mesh;
		//	private Handle3D handleA, handleB;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			mesh = new ImmediateMesh();
			var meshInstance = new MeshInstance3D
			{
				Mesh = mesh
			};
			AddChild(meshInstance);
			//handleA = new Handle3D();
			//handleB = new Handle3D();
			//AddChild(handleA);
			//AddChild(handleB);
			//handleA.Position = PointA;
			//handleB.Position = PointB;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			mesh.ClearSurfaces();
			mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
			mesh.SurfaceSetMaterial(0, new StandardMaterial3D
			{
				AlbedoColor = Color,
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
				PointSize = LineWidth * 2,
				Transparency = Color.A < 1 ? BaseMaterial3D.TransparencyEnum.Alpha : BaseMaterial3D.TransparencyEnum.Disabled
			});
			mesh.SurfaceSetColor(Color);
			mesh.SurfaceAddVertex(PointA);
			mesh.SurfaceAddVertex(PointB);
			mesh.SurfaceEnd();
		}
	}
}
#endif
