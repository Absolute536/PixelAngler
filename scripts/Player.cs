namespace GamePlayer;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldInfo;
// using WorldInfo;

public partial class Player : CharacterBody2D
{
	[Export] public AnimatedSprite2D AnimationPlayer;
	[Export] public AudioStreamPlayer2D AudioPlayer;
	[Export] Label DebugText;

	[Export] public float Speed = 60.0f;

	private Vector2 oldPosition;
	private Vector2 oldInputVector;
	// Testing
	public override void _Ready()
	{
		oldPosition = Position;
		oldInputVector = Vector2.Zero;
		// Relocate();
	}

	public delegate TileType PositionChangedEventHandler(Vector2I worldCoordinate);
	public PositionChangedEventHandler PositionChange;

	public void Relocate()
	{
		/* Extracted from the removed methods for note-keeping
		 * The root node of the player is at the same Position as the World (root) node in the main scene
		 * So, the Position of the player and its Global Position is the same (idk if it's really like that)
		 * So the Position of the playe can be rather naturally translated to the coordinates in the world tile map layer
		 * A division of 16 pixel is needed because each tile is 16 by 16 pixels in the tile map layer
		 * Player sprite is also 16 pixels wide
		 * 1 pixel displacement in the world tile map = 16 pixel displacement in the global scene
		 * Opposite: 1 displacement in global scene = 1 / 16 displacement in world tile map coordinate
		 * So need to divide the Position by 16 to map the global coordinate to the internal coordinate of the tile map layer
		 * Then we can use the internal coordinate to obtain tile information, etc...
		 */

		// BUG: Negative value on the y axis will cause misidentification by 1 tile downwards (x also?)
		// Negative values were offset by 1 tile lower/left because of -0.0xxx values get truncated to 0
		// So if -ve values, need to - 1 to move 1 tile upwards/left to ensure the coords point correctly
		// Need to check the Position itself before casting, otherwise, -1 becomes 0
		float x = Position.X / 16;
		x = x < 0 ? x - 1 : x;

		float y = Position.Y / 16;
		y = y < 0 ? y - 1 : y;

		DebugText.Text = PositionChange(new Vector2I((int) x, (int) y)).ToString();
	}


	/*
	 * PROBLEMS
	 *
	 * = Screen Tearing =
	 * - Enable V-Sync at the cost of input delay
	 * - Sync FPS to Refresh Rate of Monitor
	 *
	 * The jitter happens because of the player position being in sub-pixel position, following MoveAndSlide()
	 * In straight directions, this isn't an issue because the effects are not noticable
	 * In diagonal directions, the player sprite pixel will kinda snap along the movement, not following the grid
	 * When camera is fixed (not moving), the jitter will appear on the sprite itself (kinda oscillating)
	 * When camera can move, the jitter will appear "on" the camera instead, causing the view to be shaky
	 * sub-pixel position = camera moves "in between" a pixel, causing this sort of shakiness (not pixel perfect)

	 * The de-sync between physics tick and fps and refresh rate can also cause this
	 * physics & frame rate mismatch -> jitter because game want to got faster/slower, but movement "drags" behind?
	 * frame rate & refresh rate mismatch -> cause screen tearing (has jitter as well?)
	 * physics & refresh rate mismatch -> jitter because physics processing is slower than visual (same as the fps thingy)
	 * So either you sync everything or you enable physics interpolation

	 * Since our movement is within physics process, the problem lies on the player's position
	 * As the camera is a child of Player
	 * So I think we need to fix the Player's jitter

	 * = Camera Jitter (Player character actually, because Camera is a child of the character)
	 * - Sync Physics Tick to FPS -> may have inconsistent physics tick (not good)
	 * - Round the position of the Player node -> works, but diagonal movements are slowed
	 * - Round Global Position of camera itself -> can work, but introduces a gradual offset of camera position
	 * - Enable Physics Interpolation -> kinda works, but there is a little input delay (just tiny)

	 * I think we can do these:
	 * - Provide V-Sync toggle in settings -> ON to eliminate tearing, OFF to minimise input delay
	 * - Provide fps toggle (60 and 120) -> sync physics tick with fps only for these two options [X]
	 * - Enable physics interpolation
	 * - Round the Player position
	 * Rounding and physics interpolation produces the most effect
	 OK, so
	 - if refresh rate and fps mismatch (more precisely, lower fps than rr), it may cause jitter -> V-Sync can fix this, but increased input lag
	 - if physics tick and fps mismatch, it can cause jitter -> physics interpolation can fix automatically, but increased input lag
	 - the jitter in our case comes from the player position itself being sub-pixel, as camera is attached to player

	 FUCK IT, let's do
	 - Player movement in PhysicsProcess
	 - Camera interpolation in Process manually (goodbye time)


	 IN CONCLUSION, this shit is kinda fucked
	 - if refresh rate is low, there are some sort of jitter even if FPS matches
	 - FPS higher than physics tick, there will be jitter, but can be solved by physics interpolation
	 - FPS higher than refresh rate, no problem
	 - FPS lower than refresh rate, jitter occurs because the monitor is displaying the same frame for longer time
	 - If camera is fixed, jitter occurs due to the sub-pixel position of the player on diagonal movement
	 - If camera moves with the player, the camera itself will shake instead (due to the above reason)
	 
	 SO, WE DO THESE
	 - Get refresh rate on game start, set max fps to refresh rate or 120 fps if the ScreenGetRefreshRate() fails
	 - Enable Physics Interpolation -> to ensure physics tick and fps difference won't cause jitter
	 - Rounding the position (according to Reddit solution) to eliminate jitter because of the sub-pixel position on diagonal movement

	 https://www.reddit.com/r/godot/comments/16lft93/fix_for_pixelperfect_diagonal_movement_causing/
	 https://www.reddit.com/r/godot/comments/157z5ok/how_do_i_make_diagonal_kinrmaticbody2d_movement/
	 https://docs.godotengine.org/en/4.4/tutorials/math/interpolation.html
	 Lerp Smoothing is broken video
	 */

	public override void _PhysicsProcess(double delta)
	{
		// Removed the duplicated old stuffs
		// Leave it empty without commenting it out first, just in case there're things to be done here
	}

	List<Vector2> polyPoints = [new Vector2(0, 0)];
	Line2D l = new();

	private void CastLine()
	{

		if (Input.IsActionJustPressed("ItemAction"))
		{
			l.Name = "FishingLine";
			l.Width = 1.0f;
			l.DefaultColor = Colors.White;
			l.Points = polyPoints.ToArray();
			AddChild(l);
			Console.WriteLine("FIshing Line created on pressed");

		}

		int count = 1;
		if (Input.IsActionPressed("ItemAction"))
		{
			// Line2D line = new Line2D();
			// Vector2[] points = [new Vector2(0, 0), new Vector2(10.0f, -10.0f), new Vector2(20.0f, -20.0f), new Vector2(30.0f, -10.0f), new Vector2(40.0f, -5.0f), new Vector2(-10.0f, 10.0f)];
			// line.Points = points;
			// line.DefaultColor = Colors.White;
			// line.Width = 1.0f;
			// line.JointMode = Line2D.LineJointMode.Round;
			// AddChild(line);

			polyPoints.Add(new Vector2(count, -count));
			Line2D fishingLine = GetNode<Line2D>("FishingLine");
			fishingLine.Points = polyPoints.ToArray();
			count++;
			Console.WriteLine("Fishing Line extending on hold");

		}

		if (Input.IsActionJustReleased("ItemAction"))
		{
			// var fish = GD.Load<PackedScene>("res://scenes/Fish.tscn");
			// CharacterBody2D fishInstance = fish.Instantiate() as CharacterBody2D;
			// fishInstance.Position = l.Points[l.Points.Length - 1];
			// AddChild(fishInstance);
		}
	}
}
