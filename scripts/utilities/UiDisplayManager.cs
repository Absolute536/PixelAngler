using Godot;

public partial class UiDisplayManager : Node
{
    public static UiDisplayManager Instance {get; private set;}

    public bool CanSelectBait = true; // ok, again, this is just another quick, dirty band aid to ensure player can't change bait during fishing TAT
    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always; // always process
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        FishCatalogueUi catalogue = GetNode<FishCatalogueUi>("/root/Main/HUD/FishCatalogue");
        PauseMenu pauseMenu = GetNode<PauseMenu>("/root/Main/HUD/PauseMenu");
        // use if-statements should be fine cuz there're only 2
        // Ok... now it's growing (dang it)
        if (@event.IsActionPressed("ShowCatalogue"))
        {
            if (!pauseMenu.PauseMenuContainer.Visible) // only show catalogue if pause menu is not shown
                catalogue.ShowHideCatalogue();
        }
        else if (@event.IsActionPressed("ShowPause"))
            pauseMenu.ShowHidePauseMenu();
        else if (@event.IsActionPressed("PreviousItem") || @event.IsActionPressed("NextItem"))
        {
            if (!GetTree().Paused && CanSelectBait) // not if the scene is not paused
            {
                BaitSelectorUi baitSelector = GetNode<BaitSelectorUi>("/root/Main/HUD/BaitSelector");
                if (@event.IsAction("PreviousItem"))
                    baitSelector.ToPreviousBait();
                else
                    baitSelector.ToNextBait();
                
                AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonClick, false);
            }
        }
    }
}