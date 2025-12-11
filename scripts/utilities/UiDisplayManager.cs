using Godot;

public partial class UiDisplayManager : Node
{
    public static UiDisplayManager Instance {get; private set;}

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always; // always process
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // use if-statements should be fine cuz there're only 2
        if (@event.IsActionPressed("ShowCatalogue"))
        {
            FishCatalogueUi catalogue = GetNode<FishCatalogueUi>("/root/Main/HUD/FishCatalogue");
            catalogue.ShowHideCatalogue();
        }
        else if (@event.IsActionPressed("ShowPause"))
        {
            PauseMenu pauseMenu = GetNode<PauseMenu>("/root/Main/HUD/PauseMenu");
            pauseMenu.ShowHidePauseMenu();
        }
    }
}