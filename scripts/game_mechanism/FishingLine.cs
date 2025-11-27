using GamePlayer;
using Godot;
using System;
using System.Collections.Generic;

public partial class FishingLine : Line2D
{
    [Export] public Player Player;
	[Export] public Bobber TraceTarget; // End point of the line - the bobber
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
	}

    public override void _PhysicsProcess(double delta)
    {
        UpdateFishingLine(TraceTarget.Position);
    }

    // And different from the bobber, we wouldn't want to disable the physics process yet?
    // then bobber shouldn't be disabled as well?
    // or maybe we need to use MoveToward() in the bobber when attaching it to a fish?

    private void UpdateFishingLine(Vector2 endPosition)
    {
        float t = 0.0f;

        Vector2 controlPointStart = AnchorPoint;
        Vector2 controlPointEnd = endPosition;

        // Do we need to account for the facing direction?
        // But since we use local position for this, the anchor will always be at X = 0 (the origin relative to parent)
        // So it works here, cuz X = 0, but if we want to offset more on the horizontal axis, facing direction is important
        // Just try this for now and see
        // float controlCentreX = controlPointEnd.X < 0 ? controlPointStart.X - 16 : controlPointStart.X + 16;
        Vector2 controlPointCentre = new Vector2(controlPointStart.X, controlPointEnd.Y);
        // nevermind, just use controlStart.X as the X, else it would shift on straight line
        // OR...... we can include more logic to test the casting direction and adjust accordingly
        // Let's see first
        // need to recalculate anchor point if we want some offset from the origin (local)

        List<Vector2> pointsOnLine = [];

        // Increment t by 0.05 until 1, so there should be 20 points along the bezier curve
        // 09/11/2025: correction, should have 21 points instead (20 points + 1 at the end, at t = 1.00)
        // replaced with a for-loop instead
        for (int i = 0; i < 21; i++)
        {
            Vector2 lerpStartToCentre = controlPointStart.Lerp(controlPointCentre, t);
            Vector2 lerpCentreToEnd = controlPointCentre.Lerp(controlPointEnd, t);
            Vector2 pointOnCurve = lerpStartToCentre.Lerp(lerpCentreToEnd, t);

            pointsOnLine.Add(pointOnCurve);
            t += 0.05f;
        }

        Points = pointsOnLine.ToArray();
    }

    public void InitiateFishingLine()
    {
        Points = [AnchorPoint];
        Visible = true;
        SetPhysicsProcess(true);
    }

    public void TerminateFishingLine()
    {
        SetPhysicsProcess(false);
        Visible = false;
    }

}