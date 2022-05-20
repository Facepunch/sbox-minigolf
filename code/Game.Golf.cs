using Sandbox;

using Facepunch.Minigolf.Entities;
using Facepunch.Minigolf.UI;

namespace Facepunch.Minigolf;

public partial class Game
{
	[Net] public Course Course { get; set; }

	public void OnBallStoppedMoving(Ball ball)
	{
		// if ( CheckBounds && !ball.Cupped && !Course.CurrentHole.InBounds(ball) )
		// 	BallOutOfBounds(ball, OutOfBoundsType.Normal);
	}


	/// <summary>
	/// Called from the HoleGoal entity 
	/// </summary>
	/// <param name="ball"></param>
	/// <param name="hole"></param>
	public void CupBall( Ball ball, int hole )
    {
		Host.AssertServer();

		// Make sure the hole they cupped in is the current one...
		if ( hole != Course.CurrentHole.Number )
		{
			ResetBall( ball.Client );
			return;
		}

		GameServices.RecordEvent( ball.Client, $"Cupped hole { Course.CurrentHole.Number }", ball.Client.GetPar() );

		// Tell the ball entity it has been cupped, stops input and does fx.
		ball.Cup();

		// Let all players know the ball has been cupped.
		CuppedBall( To.Everyone, ball, ball.Client.GetPar() );

		// Boop
		if ( ball.Client.GetPar() == 1 )
		{
			Particles.Create( "particles/gameplay/hole_in_one/v2/hole_in_one.vpcf", Course.CurrentHole.GoalPosition + Vector3.Up * 16.0f );
		}
		else
		{
			Particles.Create( "particles/gameplay/hole_effect/confetti.vpcf", Course.CurrentHole.GoalPosition + Vector3.Up * 16.0f );
		}

		// Remove the ball after a few seconds.
		delayedDeletePawn();
		async void delayedDeletePawn()
		{
			await ball.Task.DelaySeconds( 4.5f );

			// Make sure our ball is still valid, maybe they disconnected in those 3 seconds.
			if ( !ball.IsValid() )
				return;

			ball.Client.Pawn = null;
			ball.Delete();
		}
	}

	[ClientRpc]
	protected void CuppedBall( Ball ball, int score )
	{
		var client = ball.Client;
		ScoreFeed.Instance.AddEntry( client, Course.CurrentHole.Par, score );

		if ( Local.Client == client )
			ParScreen.Show( Course.CurrentHole.Number, Course.CurrentHole.Par, score );
	}

	protected void ResetBall( Client cl )
	{
		if ( IsClient )
			return;

		var SpawnPosition = Course.CurrentHole.SpawnPosition;
		var SpawnAngles = Course.CurrentHole.SpawnAngles;

		if ( cl.Pawn.IsValid() && cl.Pawn is Ball ball )
		{
			if ( !ball.LastPosition.IsNearlyZero() && !ball.LastPosition.IsNaN )
			{
				SpawnPosition = ball.LastPosition;
				SpawnAngles = ball.LastAngles;
			}

			cl.Pawn.Delete();
		}

		cl.Pawn = new Ball();
		(cl.Pawn as Ball).ResetPosition( SpawnPosition, SpawnAngles );
	}

	// fuck it do this somewhere else and keep score?
	[ConCmd.Server]
	public static void Stroke( float yaw, float power )
	{
		var client = ConsoleSystem.Caller;
		if ( client == null ) return;

		if ( ConsoleSystem.Caller.Pawn is not Ball ball )
			return;

		if ( ball.InPlay )
			return;

		client.AddPar();

		ball.Stroke( Angles.AngleVector( new Angles( 0, yaw, 0 ) ), power );
	}
}
