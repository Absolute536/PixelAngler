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
        TextureHover = GD.Load<Texture2D>("res://assets/ui_design/fish_selection_button_hover.png");
        TextureNormal = GD.Load<Texture2D>("res://assets/ui_design/fish_selection_button_normal.png");
        TexturePressed = GD.Load<Texture2D>("res://assets/ui_design/fish_selection_button_press.png");
        
        AddChild(IconSprite);
        IconSprite.StretchMode = TextureRect.StretchModeEnum.KeepCentered;
        IconSprite.ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
        IconSprite.SetAnchorsAndOffsetsPreset(LayoutPreset.Center, LayoutPresetMode.Minsize);
    }

    
}