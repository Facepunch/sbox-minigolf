namespace Facepunch.Minigolf.Entities;

[Library( "minigolf_out_of_bounds" )]
[HammerEntity, Solid, AutoApplyMaterial( "materials/editor/minigolf_wall/minigolf_out_of_bounds.vmat" ), VisGroup( VisGroup.Trigger )]
[Title( "Hole Out Of Bounds" )]
public partial class OutOfBoundsArea : BaseTrigger
{
	/// <summary>
	/// When the ball enters this out of bounds area, how much time until we declare out of bounds?
	/// </summary>
	[Property(Title = "Forgiveness Time")]
	public int ForgiveTime { get; set; } = 3;

	public IEnumerable<Ball> TouchingBalls => touchingBalls;
	private readonly List<Ball> touchingBalls = new();

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is Ball ball )
		{
			AddTouchingBall( ball );

			// TODO: forgiveness time
			Game.Current.BallOutOfBounds( ball, Game.OutOfBoundsType.Normal );
		}
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( other is not Ball )
			return;

		var ball = other as Ball;

		if ( touchingBalls.Contains( ball ) )
			touchingBalls.Remove( ball );
	}

	protected void AddTouchingBall( Ball ball )
	{
		if ( !ball.IsValid() )
			return;

		if ( !touchingBalls.Contains( ball ) )
			touchingBalls.Add( ball );
	}
}
