using GamePlayer;
using Godot;
using System;

public partial class BaitSelectorUi : Control
{
	[Export] public TextureButton PreviousBait;
	[Export] public TextureButton NextBait;
	[Export] public TextureRect SelectedBaitDisplay;
	[Export] public Label BaitName;
	[Export] public Timer DisplayTimer;

	[Export] public FishingBait[] FishingBaitsAvailable;

	private int _selectedBaitIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        _selectedBaitIndex = 0;
		SelectedBaitDisplay.Texture = FishingBaitsAvailable[_selectedBaitIndex].BaitTexture;
		PreviousBait.Pressed += ToPreviousBait;
		NextBait.Pressed += ToNextBait;
		DisplayTimer.Timeout += () => {BaitName.Visible = false;};

		UpdatePlayerSelectedBait(0); // update selected bait (or else it can be null)
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }

	public void ToPreviousBait()
    {
		if (UiDisplayManager.Instance.CanSelectBait)
		{
			if (_selectedBaitIndex == 0)
				_selectedBaitIndex = FishingBaitsAvailable.Length - 1;
			else
				_selectedBaitIndex -= 1;
			
			UpdateBaitInformation();
		}

    }

	public void ToNextBait()
    {
		if (UiDisplayManager.Instance.CanSelectBait)
		{
			if (_selectedBaitIndex == FishingBaitsAvailable.Length - 1)
				_selectedBaitIndex = 0;
			else
				_selectedBaitIndex += 1;
			
			UpdateBaitInformation();
		}
    }

	private void UpdateBaitInformation()
    {
		UpdatePlayerSelectedBait(_selectedBaitIndex);
	
		SelectedBaitDisplay.Texture = FishingBaitsAvailable[_selectedBaitIndex].BaitTexture;
        BaitName.Text = FishingBaitsAvailable[_selectedBaitIndex].BaitName;
		BaitName.Visible = true;
		DisplayTimer.Start(); // it will reset the timer if started already, so this works
    }

	private void UpdatePlayerSelectedBait(int selection)
	{
		Player player = GetTree().GetFirstNodeInGroup("Player") as Player;

		// Ok ok ok. This is dirty but I guess it works
		Bobber bobber = player.GetChild(5).GetChild(2) as Bobber; // 6th child of player is ActionUI, then 3rd child of ActionUI is bobber
		bobber.SelectedBait = FishingBaitsAvailable[selection];
		GD.Print(bobber.SelectedBait.BaitName);
	}
}
