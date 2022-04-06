using Sandbox;
using System.ComponentModel.DataAnnotations;

namespace Facepunch.Minigolf.Entities;

[Library( "minigolf_flag_base")]
[Hammer.EditorModel( "models/minigolf_flag.vmdl" )]
[Display( Name = "Minigolf Flag" )]
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
