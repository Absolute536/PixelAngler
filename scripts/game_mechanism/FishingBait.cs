using Godot;

[GlobalClass]
public partial class FishingBait : Resource
{
    [Export] public string BaitName;
    [Export] public FishDiet BaitType;
    [Export] public SizeClass BaitSize;
    [Export] public Texture2D BaitTexture;
}