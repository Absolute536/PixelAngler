using Godot;
using SignalBusNS;
using System;

public partial class DayNightCycleUi : Control
{
	[Export] public RichTextLabel DayLabel;
	[Export] public RichTextLabel TimeLabel;
	[Export] public TextureRect DayNightIcon;

	// test
	// [Export] public Button button;

    public override void _Ready()
    {
        // button.Pressed += () => {UpdateTime(12, 21, 25);}; // ok it works
		SignalBus.Instance.InGameTimeChanged += UpdateTime;
    }


	public void UpdateTime(float totalMinutes, int day, int hours, int minutes)
    {
		DayLabel.Text = "Day " + (day + 1);
		TimeLabel.Text = To12Hours(hours) + ":" + FormatMinutes(minutes) + GetAmFm(hours);
		DayNightIcon.Texture = UpdateIcon(hours);
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
        // 0300 - 0659 -> Dawn
		// 0700 - 1659 -> Day
		// 1700 - 2059 -> Dusk
		// 2100 - 0259 -> Night

		if (hours >= 7 && hours <= 16)
			return GD.Load<CompressedTexture2D>("res://assets/day_icon.png");
		else if (hours >= 21 || hours <= 2)
			return GD.Load<CompressedTexture2D>("res://assets/night_icon.png");
		else if (hours >= 3 && hours <= 6)
			return GD.Load<CompressedTexture2D>("res://assets/dawn_icon.png");
		else
			return GD.Load<CompressedTexture2D>("res://assets/dusk_icon.png");
    }

}


