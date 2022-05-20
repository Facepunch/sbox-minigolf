namespace Facepunch.Minigolf.Entities;

/// <summary>
/// Where the ball spawns, also defines the hole name and par.
/// </summary>
[Library( "minigolf_ball_spawnpoint" ), HammerEntity]
[EditorModel( "models/golf_ball.vmdl" ), DrawAngles]
[Title( "Ball Spawnpoint" )]
public partial class BallSpawnpoint : Entity
{
	/// <summary>
	/// What hole number this spawnpoint belongs to.
	/// </summary>
	[Property, Net]
	public int HoleNumber { get; set; }

	/// <summary>
	/// The name of this hole, this is displayed in-game.
	/// </summary>
	[Property, Net]
	public string HoleName { get; set; } = "Untitled Hole";

	/// <summary>
	/// How many strokes should this hole be done in.
	/// </summary>
	[Property, Net]
	public int HolePar { get; set; } = 3;
}
