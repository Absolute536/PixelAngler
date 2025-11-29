using Godot;
using SignalBusNS;
using System;

public partial class DayNightTint : CanvasModulate
{
	[Export] public GradientTexture1D DayNightGradient;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        SignalBus.Instance.InGameTimeChanged += UpdateDayNightTint;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void UpdateDayNightTint(float totalMinutes, int day, int hours, int minutes)
    {
		// using Sin function/wave to measure the ratio from which the gradient is sampled
        float sampleRatio = (float) ((Mathf.Sin(totalMinutes + 0.5 * Mathf.Pi) + 1.0) / 2.0);
		Color = DayNightGradient.Gradient.Sample(sampleRatio);
    }
}
