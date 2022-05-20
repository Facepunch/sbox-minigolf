namespace Facepunch.Minigolf.Entities;

/// <summary>
/// Minigolf hole goal trigger
/// </summary>
[Library( "minigolf_hole_goal" )]
[HammerEntity, Solid, AutoApplyMaterial, VisGroup( VisGroup.Trigger )]
[Title( "Hole Goal" )]
public partial class HoleGoal : ModelEntity
{
	/// <summary>
	/// Which hole this hole is on.
	/// </summary>
	[Property, Net]
	public int HoleNumber { get; set; }

	public Particles HoleParticle { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		SetupPhysicsFromModel(PhysicsMotionType.Static);
		CollisionGroup = CollisionGroup.Trigger;
		EnableSolidCollisions = false;
		EnableTouch = true;
		EnableDrawing = true;
		Transmit = TransmitType.Always;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		HoleParticle = Particles.Create( "particles/gameplay/flag_number/flag_number.vpcf", this );

		var number = HoleNumber;

		HoleParticle.SetPositionComponent( 21, 2, number % 10 );

		number /= 10;
		HoleParticle.SetPositionComponent(21, 1, number % 10 );
	}

	public override void StartTouch( Entity other )
	{
		if ( IsClient ) return;

		if ( other is not Ball ball )
			return;

		Game.Current.CupBall( ball, HoleNumber );
	}
}
