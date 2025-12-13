using Godot;
using System;

public partial class FishingProgressBar : ProgressBar
{

	private GradientTexture1D _progressFillGradient = GD.Load<GradientTexture1D>("res://resources/ui_styles/fishing_progress_gradient_texture_1d.tres");
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// SelfModulate = new Color(214 / 255, 174 / 255, 85 / 255, 1);
		Value = MaxValue / 5; // 25% filled on instantiation
		Size = new Vector2(50, 3);
		// fillColor = GD.Load<StyleBoxFlat>("res://resources/shaders/fishing_progress_fill.tres");
		
		// initialFill.BgColor = new Color(42 / 255, 165 / 255, 101 / 255, 1);
		

		// AddThemeStyleboxOverride("fill", initialFill); // "fill", not "Fill" wth?
		// fillColor = GetThemeStylebox("Fill", "ProgressBar") as StyleBoxFlat;
		// fillColor.BgColor = new Color(42 / 255, 165 / 255, 101 / 255, 1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }


    public override void _PhysicsProcess(double delta)
    {
		StyleBoxFlat updateFill = GetThemeStylebox("fill") as StyleBoxFlat;
		float ratio = (float) (Value / MaxValue);
        // 191, 51, 59 for the RED
		// 42, 165, 101 for the GREEN
		// 213, 174, 85 for the YELLOW (?)
		
		updateFill.BgColor = _progressFillGradient.Gradient.Sample(ratio);
    }

    public override void _ExitTree()
    {
        
    }


}
