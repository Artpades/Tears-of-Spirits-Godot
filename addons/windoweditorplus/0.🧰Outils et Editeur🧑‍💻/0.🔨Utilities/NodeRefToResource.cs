using Godot;
using System;
[GlobalClass, Tool]
public partial class NodeRefToResource : Node
{
	[Export] public ColorRect NodeRect;
	[Export] ResourceNodeRefGetter ResourceNodeRefGetter;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (ResourceNodeRefGetter != null)
		{
			ResourceNodeRefGetter.NodeRect = NodeRect;
			if (NodeRect == null) GD.PrintErr("NodeRect is not assigned in ", Name);
			else GD.Print("NodeRefToResource is ready and has assigned references in ", Name);
			if (ResourceNodeRefGetter == null) GD.PrintErr("ResourceNodeRefGetter is not assigned in ", Name);
			else GD.Print("NodeRefToResource is ready and has assigned references in ", Name);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
