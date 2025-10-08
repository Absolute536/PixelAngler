using System.Collections;
using GamePlayer;
using Godot;

public class FishingRodAction : ItemAction
{
    private const int MaxLength = 80;
    private int castLength = 0;
    private Sprite2D castMarker = new Sprite2D();
    private Timer castTimer = new Timer()
    {
        ProcessCallback = Timer.TimerProcessCallback.Physics,
        // IgnoreTimeScale = true
    };
    private Line2D markerLine; // or fishing line actually

    private Player player;

    public FishingRodAction(Player player)
    {
        this.player = player;

        Image markerImage = Image.LoadFromFile("res://icon.svg");
        castMarker.Texture = ImageTexture.CreateFromImage(markerImage);
        castMarker.Visible = false;
        castMarker.Name = "CastMarker";
        castMarker.Scale = new Vector2(0.25f, 0.25f);

        castTimer.Timeout += IncreaseCastMarker; // on time out, call the function to increase the cast marker's distance from player
    }


    public void StartAction()
    {
        castTimer.Start(0.5);
    }

    private void IncreaseCastMarker()
    {
        if (player != null && castLength < MaxLength)
        {
            castLength += 16;

            if (player.FacingDirection == Vector2.Up)
                castMarker.Position = player.Position + new Vector2(0, -castLength);
            else if (player.FacingDirection == Vector2.Down)
                castMarker.Position = player.Position + new Vector2(0, castLength);
            else if (player.FacingDirection == Vector2.Right)
                castMarker.Position = player.Position + new Vector2(castLength, 0);
            else
                castMarker.Position = player.Position + new Vector2(-castLength, 0);
        }
    }

    public void EndAction()
    {
        castLength = 0; // reset cast length
    }

    // Maybe the input handling for fishing gameplay can be done within the fish object/scene itself?
}