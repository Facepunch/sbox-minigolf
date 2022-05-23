namespace Facepunch.Minigolf.Entities;

/// <summary>
/// A trigger brush that applies a constant velocity to golf balls.
/// </summary>
[Library( "minigolf_speed_boost" )]
[HammerEntity, Solid, DrawAngles( nameof( Direction ) ), AutoApplyMaterial]
[Title( "Speed Boost" )]
public partial class SpeedBoost : ModelEntity
{
	/// <summary>
	/// How much velocity will be applied across each second. This is divided and applied each tick.
	/// </summary>
	[Property]
	public float Acceleration { get; set; } = 100.0f;

	/// <summary>
	/// If the ball is going faster then this (towards the direction vector), don't apply the acceleration.
	/// Set to 0 to disable.
	/// </summary>
	[Property]
	public float MaxVelocity { get; set; } = 1000.0f;

	/// <summary>
	/// The direction the ball will move, when told to.
	/// </summary>
	[Property( Title = "Direction (Pitch Yaw Roll)" )]
	public Angles Direction { get; set; }

	public List<Ball> Balls = new();

	public override void Spawn()
	{
		base.Spawn();

		SetupPhysicsFromModel( PhysicsMotionType.Static );
		CollisionGroup = CollisionGroup.Trigger;
		EnableSolidCollisions = false;
		EnableTouch = true;
		EnableDrawing = false;
		Transmit = TransmitType.Never;
	}

	public override void StartTouch( Entity other )
	{
		if ( other is not Ball ball ) return;
		if ( Balls.Contains( ball ) ) return;
		Balls.Add( ball );
	}

	public override void EndTouch( Entity other )
	{
		if ( other is not Ball ball ) return;
		if ( !Balls.Contains( ball ) ) return;
		Balls.Remove( ball );
	}

	[Event.Physics.PreStep]
	public void ApplyForce()
	{
		var acceleration = Acceleration / ( Global.TickRate * Global.PhysicsSubSteps );

		foreach( var ball in Balls )
		{
			if ( MaxVelocity != 0 && ball.Velocity.Length >= MaxVelocity ) continue;
			ball.Velocity += Direction.Direction * acceleration;
		}
	}
}
