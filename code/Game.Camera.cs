using Sandbox;
using System.Linq;

using Facepunch.Minigolf.Entities;
using Facepunch.Minigolf.UI;

namespace Facepunch.Minigolf;

public partial class Game
{
	Entity StartCameraEntity { get; set; }

	public override CameraMode FindActiveCamera()
	{
		// If the game hasn't started yet show our "cinematic" starting camera
		if ( State == GameState.WaitingForPlayers )
		{
			if ( !StartCameraEntity.IsValid() ) StartCameraEntity = Entity.All.OfType<StartCamera>().First();
			return StartCameraEntity.Components.Get<CameraMode>();
		}

		// Pawn will only be set whilst we're in play ( e.g not spectating, or in between holes )
		if ( Local.Pawn is Ball ball )
        {
			if ( FreeCamera != null )
			{
				if ( !ball.InPlay && !ball.Cupped && FreeCamTimeLeft > 0.0f ) return FreeCamera;

				// Bad conditions, set to null and fall through
				FreeCamera = null;
			}

			if ( !ball.Cupped ) return ball.Camera;

			// Their ball is cupped, lets do the hole end camera cinematic
			if ( HoleEndCamera == null )
			{
				HoleEndCamera = new( Course.CurrentHole.GoalPosition );
			}

			return HoleEndCamera;
		}

		// Must be a spectator ( no ball pawn )

		// TODO: Actually spectate

		return null;
	}

	// HoleEndCamera is displayed:
	// 1. After user cups ball
	// 2. After all users have cupped ball
	// 3. On return to lobby
	HoleEndCamera HoleEndCamera;

	public FreeCamera FreeCamera { get; set; }
	public float FreeCamTimeLeft { get; set; } = 30.0f;

	[Event.Frame]
	public void TickFreeCamTimeLeft()
	{
		if ( FreeCamera != null )
			FreeCamTimeLeft -= RealTime.Delta;
	}
}
