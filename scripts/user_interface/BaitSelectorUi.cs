using Godot;
using System;

public partial class BaitSelectorUi : Control
{
	[Export] public Button PreviousBait;
	[Export] public Button NextBait;
	[Export] public TextureRect SelectedBaitDisplay;
	[Export] public Label BaitName;
	[Export] public Timer DisplayTimer;

	[Export] public FishingBait[] FishingBaitsAvailable;

	private int _selectedBaitIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        _selectedBaitIndex = 0;
		// SelectedBaitDisplay.Texture = _selectedBait.BaitTexture;
		PreviousBait.Pressed += ToPreviousBait;
		NextBait.Pressed += ToNextBait;
		DisplayTimer.Timeout += () => {BaitName.Visible = false;};
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }

	private void ToPreviousBait()
    {
        if (_selectedBaitIndex == 0)
			_selectedBaitIndex = FishingBaitsAvailable.Length - 1;
		else
			_selectedBaitIndex -= 1;
		
		UpdateBaitDisplay();
    }

	private void ToNextBait()
    {
        if (_selectedBaitIndex == FishingBaitsAvailable.Length - 1)
			_selectedBaitIndex = 0;
		else
			_selectedBaitIndex += 1;
		
		UpdateBaitDisplay();
    }

	private void UpdateBaitDisplay()
    {
		// SelectedBaitDisplay.Texture = FishingBaitsAvailable[_selectedBaitIndex].BaitTexture;

        BaitName.Text = FishingBaitsAvailable[_selectedBaitIndex].BaitName;
		BaitName.Visible = true;
		DisplayTimer.Start(); // it will reset the timer if started already, so this works
    }
}
