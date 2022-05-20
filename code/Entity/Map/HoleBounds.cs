namespace Facepunch.Minigolf.Entities;

/// <summary>
/// Bounds of a hole.
/// </summary>
[Library( "minigolf_hole_bounds" )]
[HammerEntity, Solid, AutoApplyMaterial( "materials/editor/minigolf_wall/minigolf_hole_bounds.vmat" )]
[VisGroup( VisGroup.Trigger )]
[Title( "Hole Bounds" )]
public partial class HoleBounds : BaseTrigger
{
	/// <summary>
	/// Which hole this hole is on.
	/// </summary>
	[Property]
	public int HoleNumber { get; set; }

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
