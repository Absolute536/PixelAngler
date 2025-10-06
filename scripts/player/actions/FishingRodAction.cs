using Godot;

public class FishingRodAction : ItemAction
{
    private int castLength = 0;
    private Sprite2D castMarker = new Sprite2D();
    private Line2D fishingLine;

    public FishingRodAction()
    {
        Image markerImage = Image.LoadFromFile("res://icon.svg");
        castMarker.Texture = ImageTexture.CreateFromImage(markerImage);
        castMarker.Visible = false;
        castMarker.Name = "CastMarker";
        castMarker.Scale = new Vector2(0.25f, 0.25f);
    }


    public void StartAction()
    {
        Line2D NewFishingLine = new Line2D()
        {
            Width = 1,
            DefaultColor = Colors.White,
        };
    }

    public void EndAction()
    {

    }
}