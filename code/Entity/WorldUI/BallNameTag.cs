using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Facepunch.Minigolf.Entities;

// TODO: Ideally I want the SceneObject to be parented to the ball and remain in the same position
//       And then just offset the panel when rendering it.
public partial class BallNameTag : WorldPanel
{
	Ball Owner { get; init; }

	public BallNameTag( Ball owner )
	{
		Owner = owner;

		PanelBounds = new Rect( 0, -80, 800, 160 );

		Add.Image( $"avatar:{ owner.Client.PlayerId }" );
		Add.Label( owner.Client.Name );

		StyleSheet.Load( "/Entity/WorldUI/BallNameTag.scss" );

		// Can't do this cause it'll just spin like nuts
		// owner.SceneObject.AddChild( "NameTagPanel", SceneObject );
	}

	Angles CurrentDirection;

	[Event.PreRender]
	void MoveNameTag()
	{
		// Don't tidy up here, it's the owner's responsibility
		if ( !Owner.IsValid() )
			return;

		CurrentDirection = Angles.Lerp( CurrentDirection, Owner.Direction, RealTime.Delta * 4.0f ).WithRoll(0).WithPitch(0);

		var DownTrace = Trace.Ray( Owner.Position, Owner.Position + Vector3.Down * 34.0f ).WithoutTags( "golf_ball" ).Run();

		Position = DownTrace.EndPosition + Vector3.Up * 0.1f;

		var forward = Angles.AngleVector( CurrentDirection );
		var right = Vector3.Cross( forward, Vector3.Up );

		Position += right * 8.0f;
		Rotation = Rotation.From( CurrentDirection ) * Rotation.FromYaw( 180 ) * Rotation.From( DownTrace.Normal.EulerAngles );

		if ( !DownTrace.Hit )
		{
			Style.Opacity = 0.0f;
		}
		else
		{
			if ( DownTrace.Distance > 4 )
			{
				Style.Opacity = (34.0f - DownTrace.Distance) / 30.0f;
			}
			else
			{
				Style.Opacity = 1.0f;
			}
		}
	}
}
