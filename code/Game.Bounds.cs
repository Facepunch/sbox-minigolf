using Sandbox;
using System.Collections.Generic;

using Facepunch.Minigolf.Entities;
using Facepunch.Minigolf.UI;

namespace Facepunch.Minigolf;

public partial class Game
{
	[ConVar.Server( "minigolf_check_bounds" )]
	public static bool CheckBounds { get; set; } = true;

	[ConVar.Server( "minigolf_bounds_forgiveness" )]
	public static float BoundsForgiveness { get; set; } = 3.0f;

	public enum OutOfBoundsType
	{
		Normal,
		Water,
		Fire
	}

	Dictionary<Ball, float> OutOfBoundsBalls = new();

	public void UpdateBallInBounds( Ball ball, bool inBounds )
	{
		if ( inBounds && OutOfBoundsBalls.ContainsKey( ball ) )
		{
			OutOfBoundsBalls.Remove( ball );
		}

		if ( !inBounds )
		{
			OutOfBoundsBalls[ball] = Time.Now;
		}
	}

	[Event.Tick]
	private void CheckBoundsTimes()
	{
		var copy = new Dictionary<Ball, float>(OutOfBoundsBalls);
		foreach ( var ball in copy.Keys )
		{
			if ( !ball.IsValid() || ball.Cupped )
			{
				OutOfBoundsBalls.Remove( ball );
				continue;
			}

			var time = copy[ball] + BoundsForgiveness;
			if ( Time.Now > time )
			{
				OutOfBoundsBalls.Remove( ball );
				BallOutOfBounds( ball, OutOfBoundsType.Normal );
			}
		}
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
		Local.Hud.AddChild<OutOfBounds>();
	}
}
