namespace GamePlayer;

using Godot;
using GameWorld;
using SignalBusNS;

public partial class Player : CharacterBody2D
{
	[Export] public AnimatedSprite2D AnimationPlayer;
	[Export] public AudioStreamPlayer2D AudioPlayer;
	[Export] public float Speed = 60.0f;

	[Export] public Camera2D PlayerCamera;

	private Vector2 _facingDirection = Vector2.Down; // by default face down?
	public Vector2 FacingDirection
	{
		get => _facingDirection;
		set
		{
			if (value == Vector2.Up || value == Vector2.Down)
				_facingDirection = value;
			else if (value.X > 0)
				_facingDirection = Vector2.Right;
			else
				_facingDirection = Vector2.Left;

		}
	}
	// ---------------------------------------------------------------------------------------------------


	// Equipment list
	// Currently equipped items? (or on the UI)
	// Do we need item shop here as well?

	// FOR DEBUG USE: REMOVE LATER OR HIDE IT
	// [Export] Label DebugText;

	public override void _Ready()
	{
		// initialise camera limit
		World worldTileMap = GetTree().GetFirstNodeInGroup("World") as World;
		Rect2I worldRect = worldTileMap.WorldLayer.GetUsedRect();
		// can use tileset, then get the cell size for this but we'll just use 16, 16 for convenience sake

		// I think should -1 (for right and bottom) & + 1 (for top and left) to like enforce a 1 tile padding?
		PlayerCamera.LimitLeft = (worldRect.Position.X + 1) * 16;
		PlayerCamera.LimitRight = (worldRect.End.X - 1) * 16;
		PlayerCamera.LimitTop = (worldRect.Position.Y + 1) * 16;
		PlayerCamera.LimitBottom = (worldRect.End.Y - 1) * 16;

		float initialX = SaveLoadUtil.Instance.LoadedGameSave.PlayerGlobalPositionX;
		float initialY = SaveLoadUtil.Instance.LoadedGameSave.PlayerGlobalPositionY;
		GlobalPosition = new Vector2(initialX, initialY);
	}

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
		// So if -ve values, need to - 1 to move 1 tile upwards/left to ensure the coords point correctly (not point to zero)
		// Need to check the Position itself before casting, otherwise, -1 (on the tile mpa) becomes 0 (when dividing here)

		// float x = Position.X / 16;
		// x = x < 0 ? x - 1 : x;

		// float y = Position.Y / 16;
		// y = y < 0 ? y - 1 : y;
		// DebugText.Text = PositionChange(new Vector2I((int) x, (int) y)).ToString();

		// LMAO, don't even need to convert it myself, there's a built-in function
		// DebugText.Text = PositionChanged(Position).ToString() + "\n" + LocationChanged(Position);
		// PositionEventArgs eventArgs = new PositionEventArgs()
		// {
		// 	Position = this.Position
		// };
		// DebugText.Text = SignalBus.Instance.InvokePositionChangedEvent(this, eventArgs).ToString() + "\n" + LocationChanged(Position);

		// DebugText.Text = GameInfo.Instance.GetTileType(Position).ToString() + "\n" + GameInfo.Instance.GetWorldLocation(Position).ToString();
	}

	public void SaveState()
	{
		SaveLoadUtil.Instance.LoadedGameSave.PlayerGlobalPositionX = GlobalPosition.X;
		SaveLoadUtil.Instance.LoadedGameSave.PlayerGlobalPositionY = GlobalPosition.Y;
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
}
