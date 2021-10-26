using System;
using Sandbox;

namespace Minigolf
{
	public partial class Game
	{
		[Net] public Course Course { get; set; }

		[ServerVar( "minigolf_check_bounds" )]
		public static bool CheckBounds { get; set; } = true;

        static readonly SoundEvent SoundHoleInOne = new("sounds/minigolf.crowd_ovation.vsnd");
		static readonly SoundEvent SoundBelowPar = new("sounds/minigolf.fart.vsnd");
		static readonly SoundEvent InHoleSound = new("sounds/minigolf.ball_inhole.vsnd");

		public void OnBallStoppedMoving(Ball ball)
		{
			// if ( CheckBounds && !ball.Cupped && !Course.CurrentHole.InBounds(ball) )
			// 	BallOutOfBounds(ball, OutOfBoundsType.Normal);
		}

		public enum OutOfBoundsType
		{
			Normal,
			Water,
			Fire
		}

		public void BallOutOfBounds(Ball ball, OutOfBoundsType type)
        {
			if ( IsClient )
				return;

			ResetBall( ball.Client );

			// Tell the ball owner his balls are out of bounds
			ClientBallOutOfBounds( To.Single(ball) );
		}

		[ClientRpc]
		public void ClientBallOutOfBounds()
		{
			_ = OutOfBounds.Current.Show();
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

			// Remove the ball after a few seconds.
			delayedDeletePawn();
			async void delayedDeletePawn()
			{
				await ball.Task.DelaySeconds( 3.5f );

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
			ScoreFeed.Instance.AddEntry( client, Course.CurrentHole.Par - score );

			if ( Local.Client == client )
				ParScreen.Show( Course.CurrentHole.Number, Course.CurrentHole.Par, score );
		}

		protected void ResetBall( Client cl )
		{
			if ( IsClient )
				return;

			if ( cl.Pawn.IsValid() )
				cl.Pawn.Delete();

			cl.Pawn = new Ball();
			(cl.Pawn as Ball).ResetPosition( Course.CurrentHole.SpawnPosition, Course.CurrentHole.SpawnAngles );
		}

		// fuck it do this somewhere else and keep score?
		[ServerCmd]
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
}