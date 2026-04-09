using Godot;
using System;
#if TOOLS
[GlobalClass, Tool]
#endif
public partial class DecalMarinEffect : Decal
{
	[Export] public FastNoiseLite fastNoise;
	[Export] private float offsetValue = -10000;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		offsetValue = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (fastNoise != null)
		{
			offsetValue += (float)delta * 5;
			fastNoise.Offset = new(offsetValue, offsetValue, 0f);

		}
	}
}
