using GamePlayer;
using Godot;
using System;
using System.Linq;

public partial class FishingLine : Line2D
{
    [Export] public Player Player;
	[Export] public Sprite2D TraceTarget; // End point of the line - the bobber
    [Export] public Vector2 AnchorPoint; // Anchor (First) point of the line - at the end of the fishing rod

    private Timer _addPointTimer = new Timer();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        SetProcess(false);

        _addPointTimer.WaitTime = 0.02; // every 0.02 we check and modify the line instead of every physics frame?
        AnchorPoint = new Vector2(0, -32);
        Points = [AnchorPoint];
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(double delta)
    // {
    // }

    public override void _PhysicsProcess(double delta)
    {
        // every physics frame, we track the position of the TraceTarget
        // and add the position as a new polypoint
        // need to check against last point (should be the "largest" on one axis)
        // and adjust the polypoints dynamically

        if (TraceTarget.IsPhysicsProcessing())
        {
            FishingLineAction();
        }

        // GD.Print("Fishing Line Points: " + GlobalPosition);
    }

    // And different from the bobber, we wouldn't want to disable the physics process yet?
    // then bobber shouldn't be disabled as well?
    // or maybe we need to use MoveToward() in the bobber when attaching it to a fish?
    private void FishingLineAction()
    {
        Visible = true;
        Vector2 newPoint = TraceTarget.Position;
        Vector2[] linePoints = new Vector2[Points.Length + 1];
        for (int i = 0; i < Points.Length; i++)
            linePoints[i] = Points[i];
        linePoints[linePoints.Length - 1] = newPoint;
        Points = linePoints;
    }

    private void DrawLine()
    {
        // Control Point 1 = Anchor Point
        // Control Point 2 = Global(?) Position of the Bobber in every frame
        // Control Point Centre = (CP2.X - 2, CP1.Y - 4) --> Idk how much of this is correct, but we'll see


    }

    public void StartLineAction(Vector2 endPosition)
    {
        Points = [Vector2.Zero, endPosition];
        GD.Print(Points[0] + ", " + Points[1]);
        // GD.Print("Global Line: " + GlobalPosition);
    }

}