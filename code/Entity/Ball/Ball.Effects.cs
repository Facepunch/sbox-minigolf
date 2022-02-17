using System;
using Sandbox;

namespace Facepunch.Minigolf.Entities;

public partial class Ball
{
	// Everything in here is clientside only, it's easier to manipulate effects that way.

	Particles Trail { get; set; }
	Particles Circle { get; set; }

	PowerArrow PowerArrow { get; set; }

	// TraceResult of a downwards trace run every clientside frame.
	public TraceResult DownTrace { get; set; }

	BallNameTag NameTag { get; set; }

	Material BallMaskBack = Material.Load( "materials/minigolf.ball_mask_back.vmat" );
	Material BallMaskFront = Material.Load( "materials/minigolf.ball_mask_front.vmat" );

	public override void OnNewModel( Model model )
	{
		base.OnNewModel( model );


		if ( IsClient )
		{
			NameTag = new BallNameTag( this );

			var backSceneObj = new SceneObject( model, Transform.Zero );
			backSceneObj.SetMaterialOverride( BallMaskFront );

			var frontSceneObj = new SceneObject( model, Transform.Zero );
			backSceneObj.SetMaterialOverride( BallMaskBack );

			SceneObject.AddChild( "stencil_back", backSceneObj );
			SceneObject.AddChild( "stencil_front", frontSceneObj );

		}
	}

	protected override void OnDestroy()
	{
		if ( IsClient )
		{
			NameTag?.Delete();
			PowerArrow?.Delete();
		}

		CleanupCustomization();
	}

	private void CreateParticles()
	{
		// Create all particles clientside, this gives us authority over them to set control points clientside.
		Host.AssertClient();

		var ballRadius = CollisionBounds.Size.z / 2;

		//Trail = Particles.Create( "particles/gameplay/ball_trail/ball_trail.vpcf", this );
		//Trail.SetPosition( 1, Vector3.One ); // Color

		Circle = Particles.Create( "particles/gameplay/ball_circle/ball_circle.vpcf", this );
		Circle.SetPosition( 1, Vector3.Down * ballRadius + Vector3.Up * 0.01f );
	}

	[Event.Frame]
	private void Frame()
	{
		DownTrace = Trace.Ray( Position, Position + Vector3.Down * (CollisionBounds.Size.z) ).WithoutTags( "golf_ball" ).Run();

		if ( !InPlay )
		{
			var ballRadius = CollisionBounds.Size.z / 2;

			if ( Circle == null )
			{
				Circle = Particles.Create( "particles/gameplay/ball_circle/ball_circle.vpcf", this );
				Circle.SetPosition( 1, Vector3.Down * ballRadius + Vector3.Up * 0.01f );
			}

			Circle.SetPosition( 2, DownTrace.Normal );
		}
		else
		{
			Circle?.Destroy( true );
			Circle = null;
		}

		if ( Local.Pawn == this )
		{
			AdjustArrow();
		}
	}

	private void AdjustArrow()
	{
		// Only show the arrow if we're charging a shot, delete otherwise.
		if ( ShotPower.AlmostEqual( 0 ) )
		{
			if ( PowerArrow != null )
			{
				PowerArrow.Delete();
				PowerArrow = null;
			}

			return;
		}

		if ( Game.Current.BallCamera is not FollowBallCamera camera )
			return;

		if ( !PowerArrow.IsValid() )
			PowerArrow = new();

		var direction = Angles.AngleVector( new Angles( 0, camera.Angles.yaw, 0 ) );

		var ballRadius = CollisionBounds.Size.z / 2;
		PowerArrow.Position = Position + Vector3.Down * ballRadius + Vector3.Up * 0.01f + direction * 5.0f;
		PowerArrow.Direction = direction;
		PowerArrow.Power = ShotPower;
	}
}