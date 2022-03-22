using Facepunch.Customization;
using Sandbox;

namespace Facepunch.Minigolf.Entities;

public partial class Ball
{

	private int parthash = -1;
	private Particles trail;

	[Net]
	public AnimEntity Hat { get; set; }
	private AnimEntity localhat;

	private void CleanupCustomization()
	{
		if ( IsServer )
		{
			if ( Hat.IsValid() ) Hat.Delete();
			if ( trail != null ) trail.Destroy();
		}

		if ( IsClient )
		{
			if ( localhat.IsValid() ) localhat.Delete();
		}

		Hat = null;
		trail = null;
		localhat = null;
	}

	[Event.Tick.Server]
	private void EnsureCustomization()
	{
		var cc = Client.Components.GetOrCreate<CustomizeComponent>();

		var hash = cc.GetPartsHash();
		if ( hash == parthash ) return;

		parthash = hash;
		ApplyCustomization();
	}

	private Vector3 prevPosition;
	[Event.Tick]
	private void MoveHat()
	{
		if ( !Hat.IsValid() ) return;

		if ( IsLocalPawn && !localhat.IsValid() )
		{
			Hat.RenderColor = Color.Transparent;
			localhat = new AnimEntity( Hat.GetModelName() );
		}

		var target = IsLocalPawn ? localhat : Hat;
		target.Position = Position + Vector3.Up * 2;

		var dir = Position - prevPosition;
		prevPosition = Position;

		if ( dir.IsNearlyZero() ) return;

		var targetAngles = Vector3.VectorAngle( dir );
		target.Rotation = Rotation.Lerp( target.Rotation, Rotation.From( targetAngles ), 5f * Time.Delta );
	}

	private void ApplyCustomization()
	{
		var cc = Client.Components.GetOrCreate<CustomizeComponent>();

		// todo: could use an ez mode for setting skins and models
		// especially for client/local pawn stuff like hat..

		CleanupCustomization();
		CleanupCustomizationOnClient();

		var trailpart = cc.GetEquippedPart( "Trails" );
		if ( trailpart != null )
		{
			trail = Particles.Create( trailpart.AssetPath, this );
			trail.SetPosition( 1, Vector3.One ); // Color
		}

		var hatpart = cc.GetEquippedPart( "Hats" );
		if ( hatpart != null )
		{
			Hat = new AnimEntity( hatpart.AssetPath );
		}

		var skinpart = cc.GetEquippedPart( "Skins" );
		if ( skinpart != null )
		{
			SetSkinOnClient( To.Everyone, NetworkIdent, skinpart.AssetPath );
		}
	}

	[ClientRpc]
	private void CleanupCustomizationOnClient()
	{
		CleanupCustomization();
	}

	[ClientCmd( "setskin", CanBeCalledFromServer = true )]
	public static void SetSkinOnClient( int ballIdent, string assetPath )
	{
		var ball = Entity.FindByIndex( ballIdent );
		if ( ball is not Ball b ) return;

		b.SetMaterialOverride( assetPath );
	}

}
