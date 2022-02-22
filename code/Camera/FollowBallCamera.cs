using Sandbox;
using System;

using Facepunch.Minigolf.Entities;

namespace Facepunch.Minigolf;

public class FollowBallCamera : CameraMode
{
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

		Position = Ball.Position + Vector3.Up * (24 + (Ball.CollisionBounds.Center.z * Ball.Scale));
		TargetRotation = Rotation.From( TargetAngles );

		Rotation = Rotation.Slerp( Rotation, TargetRotation, RealTime.Delta * 10.0f );
		TargetDistance = TargetDistance.LerpTo( Distance, RealTime.Delta * 5.0f );
		Position += Rotation.Backward * TargetDistance;

		FieldOfView = 80.0f;
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
}