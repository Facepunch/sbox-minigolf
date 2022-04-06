using Sandbox;
using System;
using System.ComponentModel.DataAnnotations;

namespace Facepunch.Minigolf.Entities;

[Library( "minigolf_trap" )]
[Hammer.Solid]
[Hammer.AutoApplyMaterial( "materials/editor/minigolf_wall/minigolf_trap.vmat" )]
[Hammer.VisGroup( Hammer.VisGroup.Trigger )]
[Display( Name = "Minigolf Trap" )]
public partial class Trap : ModelEntity
{

	[Property( "Particle" ), ResourceType( "vpcf" )]
	public string ParticleEffect { get; set; } = "particles/gameplay/ball_water_splash/ball_water_splash.vpcf";

	/// <summary>
	/// Name of the sound to play.
	/// </summary>
	[Property( "soundName" ), FGDType( "sound" )]
	public string SoundName { get; set; } = "minigolf.ball_in_water";

	[Property("Particle Tint" )]
	[Net] public Color ParticleTint { get; set; }= Color.Black;


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

		Sound.FromWorld( SoundName, ball.Position );
		var ParticleTrap = Particles.Create( ParticleEffect, ball.Position );
		ParticleTrap.SetPosition( 2, ParticleTint * 255 );


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
