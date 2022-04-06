using Sandbox;
using System;
using System.Collections.Generic;

using Facepunch.Minigolf.Entities;

namespace Facepunch.Minigolf;

public class FollowBallCamera : CameraMode
{
	private List<MiniProp> viewblockers = new();

	// should only need TargetRotation but I'm shit
	public Angles TargetAngles;
	Rotation TargetRotation;

	private float Distance = 150.0f;
	private float TargetDistance = 150.0f;

	public float MinDistance => 100.0f;
	public float MaxDistance => 300.0f;
	public float DistanceStep => 10.0f;

	public Ball Ball => Entity as Ball;

	public override bool CanAddToEntity(Entity entity)
	{
		// Can only work on your balls
		return entity is Ball;
	}

	public override void Build( ref CameraSetup camSetup )
	{
		base.Build( ref camSetup );
		camSetup.Position = Position;
		camSetup.Rotation = Rotation;
	}

	public override void Update()
	{
		if ( !Ball.IsValid() ) return;

		UpdateViewBlockers( Ball );

		Position = Ball.Position + Vector3.Up * (24 + (Ball.CollisionBounds.Center.z * Ball.Scale));
		TargetRotation = Rotation.From( TargetAngles );

		Rotation = Rotation.Slerp( Rotation, TargetRotation, RealTime.Delta * 10.0f );
		TargetDistance = TargetDistance.LerpTo( Distance, RealTime.Delta * 5.0f );
		Position += Rotation.Backward * TargetDistance;

		FieldOfView = 80.0f;

		var center = Ball.Position + Vector3.Up * 80;
		var distance = 150.0f * Ball.Scale;
		var targetPos = center + Input.Rotation.Forward * -distance;

		var tr = Trace.Ray( center, targetPos )
			.Ignore( Ball )
			.Radius( 8 )
			.Run();

		var endpos = tr.EndPosition;

		if ( tr.Entity is MiniProp ufp )
		{
			if ( ufp.NoCameraCollide )
				endpos = targetPos;
		}
	}

	public override void BuildInput( InputBuilder input )
	{
		// We take control of the camera when the ball is cupped.
		if ( Ball.Cupped )
			return;

		Distance = Math.Clamp( Distance + (-input.MouseWheel * DistanceStep), MinDistance, MaxDistance );

		TargetAngles.yaw += input.AnalogLook.yaw;

		if ( !input.Down( InputButton.Attack1 ) )
			TargetAngles.pitch += input.AnalogLook.pitch;

		TargetAngles = TargetAngles.Normal;

		if ( !input.Down( InputButton.Attack1 ) )
			TargetAngles.pitch = TargetAngles.pitch.Clamp( 0, 89 );
	}

	private void UpdateViewBlockers( Ball pawn )
	{
		foreach ( var ent in viewblockers )
		{
			ent.BlockingView = false;
		}
		viewblockers.Clear();

		var traces = Trace.Sphere( 3f, CurrentView.Position, pawn.Position ).RunAll();

		if ( traces == null ) return;

		foreach ( var tr in traces )
		{
			if ( tr.Entity is not MiniProp prop ) continue;
			prop.BlockingView = true;
			viewblockers.Add( prop );
		}
	}
}
