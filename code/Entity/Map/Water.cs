namespace Facepunch.Minigolf.Entities;

/// <summary>
/// Water specifically for Minigolf... Why?!
/// </summary>
[Library( "minigolf_water" )]
[HammerEntity, Solid, AutoApplyMaterial( "materials/editor/minigolf_wall/minigolf_water.vmat" ), VisGroup( VisGroup.Trigger )]
[Title( "Water ( Minigolf Specific )" )]
public partial class Water : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();

		var PhysGroup = SetupPhysicsFromModel( PhysicsMotionType.Static );
		PhysGroup?.SetSurface( "water" );

		ClearCollisionLayers();
		AddCollisionLayer( CollisionLayer.Water );
		AddCollisionLayer( CollisionLayer.Trigger );
		EnableSolidCollisions = false;
		EnableTouch = true;
		EnableTouchPersists = false;

		Transmit = TransmitType.Never;
	}

	public override void StartTouch( Entity other )
	{
		if ( other is not Ball ball )
			return;

		ball.InWater = true;

		Sound.FromWorld( "minigolf.ball_in_water", ball.Position );
		Particles.Create( "particles/gameplay/ball_water_splash/ball_water_splash.vpcf", ball.Position );

		Action task = async () =>
		{
			await Task.DelaySeconds( 2 );

			if ( !other.IsValid() )
				return;

			if ( other is Ball ball )
					Game.Current.BallOutOfBounds( ball, Game.OutOfBoundsType.Water );
		};
		task.Invoke();
	}
}
