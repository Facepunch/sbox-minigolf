using Sandbox;

namespace Facepunch.Minigolf.Entities;

[Library( "minigolf_flag_base", Description = "Minigolf Flag Base" )]
[Hammer.EditorModel( "models/minigolf_flag.vmdl" )]
public partial class FlagBase : ModelEntity
{
	static readonly Model FlagModel = Model.Load( "models/minigolf_flag.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = FlagModel;

		MoveType = MoveType.None;
		CollisionGroup = CollisionGroup.Never;
		PhysicsEnabled = false;
		UsePhysicsCollision = false;

		Transmit = TransmitType.Always;
	}
}
