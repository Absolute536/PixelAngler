using GamePlayer;
using Godot;
using System;

public partial class FishingLine : Line2D
{
    [Export] public Player Player;
	[Export] public Sprite2D TraceTarget; // End point of the line - the bobber
    [Export] public Vector2 AnchorPoint; // Anchor (First) point of the line - at the end of the fishing rod

    private Timer _addPointTimer = new Timer();

    private float _tParameter = 0;
    private const float LowLimit = 0;
    private const float UpLimit = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        SetProcess(false);
        SetPhysicsProcess(false);

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

        // if (TraceTarget.IsPhysicsProcessing())
        // {
        //     FishingLineAction();
        // }
        _tParameter += (float)delta;
        // _tParameter += 0.1f;
        // if (_tParameter <= UpLimit)
        // {
            Vector2 point = DrawLine(_tParameter);
            Vector2[] newArr = new Vector2[Points.Length + 1];
            Points.CopyTo(newArr, 0);
            newArr[Points.Length] = point;
            Points = newArr;
        // }

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

    private Vector2 endP; // testing
    private Vector2 DrawLine(float t)
    {
        // Control Point 1 = Anchor Point
        // Control Point 2 = Global(?) Position of the Bobber in every frame
        // Control Point Centre = (abs(CP2.X - 2), CP1.Y - 4) --> Idk how much of this is correct, but we'll see
        // Ok, it should be CP2.X - 2 and same Y as CP2.Y 
        Vector2 controlPoint1 = AnchorPoint;
        // Vector2 controlPoint2 = TraceTarget.Position;
        Vector2 controlPoint2 = endP;

        // Sketchy
        float controlCentreX = controlPoint2.X < 0 ? controlPoint2.X + 64 : controlPoint2.X - 64;
        Vector2 controlPointCentre = new Vector2(controlCentreX, controlPoint2.Y); // + 4 so that it goes down
        GD.Print("P0: " + controlPoint1);
        GD.Print("P2: " + controlPoint2);
        GD.Print(controlPointCentre);
        // Vector2 pointOnCurve = (1 - t) * ((1 - t) * controlPoint1 + t * controlPointCentre) + t * ((1 - t) * controlPointCentre + t * controlPoint2);
        // Vector2 pointOnCurve = (1 - t) * (1 - t) * controlPoint1 + 2 * (1 - t) * t * controlPointCentre + t * t * controlPoint2;

        // Try godot example
        Vector2 q0 = controlPoint1.Lerp(controlPointCentre, t);
        Vector2 q1 = controlPointCentre.Lerp(controlPoint2, t);
        Vector2 pointOnCurve = q0.Lerp(q1, t);

        // ok, I think it has sth to do with local position

        return pointOnCurve; // let's make it static first to verify the idea
    }

    public void StartDraw(Vector2 end)
    {
        SetPhysicsProcess(true);
        endP = end;
    }

    public void StartLineAction(Vector2 endPosition)
    {
        Points = [Vector2.Zero, endPosition];
        GD.Print(Points[0] + ", " + Points[1]);
        // GD.Print("Global Line: " + GlobalPosition);
    }

}