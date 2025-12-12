using Godot;
using SignalBusNS;
using System;

public partial class DayNightCycleUi : Control
{
	[Export] public RichTextLabel DayLabel;
	[Export] public RichTextLabel TimeLabel;
	[Export] public TextureRect DayNightIcon;
	[Export] public NinePatchRect DayNightBackground; // can change its customer minimum to make it dynamically resize

	private int _previousHours = -1; // just to reduce the icon update frequency

	// test
	// [Export] public Button button;

    public override void _Ready()
    {
        // button.Pressed += () => {UpdateTime(12, 21, 25);}; // ok it works
		SignalBus.Instance.InGameTimeChanged += UpdateTimeInfo;
    }


	public void UpdateTimeInfo(float scaledTotalMinutes, int day, int hours, int minutes)
    {
		DayLabel.Text = "Day " + (day + 1); // So that first day is 0 + 1 = 1

		TimeLabel.Text = To12Hours(hours) + ":" + FormatMinutes(minutes) + GetAmFm(hours);

		if (_previousHours != hours)
        {
            _previousHours = hours;
			DayNightIcon.Texture = UpdateIcon(hours);
        }
    }

	private string To12Hours(int hours)
    {
		// 0 = 12am
        if (hours == 0)
			return "12";
		
		// adjust to am
		if (hours > 12)
			return (hours - 12).ToString();
		
		return hours.ToString();
    }

	private string FormatMinutes(int minutes)
    {
        if (minutes < 10)
			return "0" + minutes.ToString();
		
		return minutes.ToString();
    }

	private string GetAmFm(int hours)
    {
        if (hours < 12)
			return "AM";
		else
			return "PM";
    }

	private CompressedTexture2D UpdateIcon (int hours)
    {
        // 0400 - 0759 -> Dawn (4 hours)
		// 0800 - 1559 -> Day (8 hours)
		// 1600 - 1959 -> Dusk (4 hours)
		// 2000 - 0359 -> Night (8 hours)

		if (hours >= 8 && hours <= 15)
			return GD.Load<CompressedTexture2D>("res://assets/ui_design/day_icon.png");
		else if (hours >= 20 || hours <= 3)
			return GD.Load<CompressedTexture2D>("res://assets/ui_design/night_icon.png");
		else if (hours >= 4 && hours <= 7)
			return GD.Load<CompressedTexture2D>("res://assets/ui_design/dawn_icon.png");
		else
			return GD.Load<CompressedTexture2D>("res://assets/ui_design/dusk_icon.png");
    }

}


