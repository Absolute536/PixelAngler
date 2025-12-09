using Godot;
using System;

public partial class CatalogueButton : TextureButton
{
    public int ButtonId {get; set;}

    // 1 delegate is probably enough
    public delegate void FishIconPressedEventHandler(int buttonId);
    public FishIconPressedEventHandler FishSelectionChanged;
    public TextureRect IconSprite = new TextureRect();

    public override void _Ready()
    {
        // No need to unsubscribe I presume
        Pressed += () => {FishSelectionChanged?.Invoke(ButtonId);};
    }

    
}