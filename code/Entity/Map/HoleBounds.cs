using Sandbox;
using System.ComponentModel.DataAnnotations;

namespace Facepunch.Minigolf.Entities;

/// <summary>
/// Bounds of a hole.
/// </summary>
[Library( "minigolf_hole_bounds" )]
[Hammer.Solid]
[Hammer.AutoApplyMaterial( "materials/editor/minigolf_wall/minigolf_hole_bounds.vmat" )]
[Hammer.VisGroup( Hammer.VisGroup.Trigger )]
[Display( Name = "Minigolf Hole Boundaries" )]
public partial class HoleBounds : ModelEntity
{
	/// <summary>
	/// Which hole this hole is on.
	/// </summary>
	[Property]
	public int HoleNumber { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		SetupPhysicsFromModel( PhysicsMotionType.Static );
		CollisionGroup = CollisionGroup.Trigger;
		EnableSolidCollisions = false;
		EnableTouch = true;

		Transmit = TransmitType.Never;
	}

	public override void StartTouch(Entity other)
	{
		if ( Game.Current.Course.CurrentHole.Number != HoleNumber )
			return;

		if ( other is not Ball ball )
			return;

		Game.Current.UpdateBallInBounds( ball, true );
	}

	public override void EndTouch(Entity other)
	{
		if ( Game.Current.Course.CurrentHole.Number != HoleNumber )
			return;

		if ( other is not Ball ball )
			return;

		Game.Current.UpdateBallInBounds( ball, false );
	}
}
