
using Facepunch.Customization;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace Facepunch.Minigolf;

internal class CustomizeRenderScene : Panel
{

	private ScenePanel ScenePanel;
	private SceneWorld SceneWorld;
	private Angles CameraAngle;

	private int hash;

	public override void OnHotloaded()
	{
		base.OnHotloaded();

		Build();
	}

	public override void Tick()
	{
		base.Tick();

		if ( SceneWorld.IsValid() )
		{
			var trail = SceneWorld.SceneObjects.FirstOrDefault( x => x is SceneParticles ) as SceneParticles;
			if ( trail.IsValid() )
			{
				trail.Simulate( RealTime.Delta );

				var pos = new Vector3( MathF.Cos( Time.Now * 4f ) * 12f, MathF.Sin( Time.Now * 4f ) * 8f, -5f );
				trail.SetControlPoint( 0, pos );
			}
		}

		var cc = Local.Client.Components.Get<CustomizeComponent>();
		if ( cc == null ) return;

		var newhash = cc.GetPartsHash();
		if ( newhash == hash ) return;

		hash = newhash;

		Build();
	}

	private void Build()
	{
		ScenePanel?.Delete();
		ScenePanel = null;

		SceneWorld?.Delete();
		SceneWorld = new SceneWorld();

		var modelscale = 3f;
		var golfball = new SceneModel( SceneWorld, "models/golf_ball.vmdl", Transform.Zero.WithScale( modelscale ) );

		ScenePanel = Add.ScenePanel( SceneWorld, Vector3.Zero, Rotation.From( CameraAngle ), 75 );
		ScenePanel.Style.Width = Length.Percent( 100 );
		ScenePanel.Style.Height = Length.Percent( 100 );
		ScenePanel.CameraPosition = golfball.Rotation.Forward * 32f + Vector3.Up * 3f;
		ScenePanel.CameraRotation = Rotation.LookAt( golfball.Rotation.Backward ).RotateAroundAxis( Vector3.Forward, -90 );
		CameraAngle = ScenePanel.CameraRotation.Angles();

		new SceneLight( SceneWorld, ScenePanel.CameraPosition + Vector3.Up * 5 + Vector3.Right * 2, 200, Color.White );
		new SceneLight( SceneWorld, Vector3.Down * 50 + Vector3.Left * 20, 200, Color.White.Darken( .25f ) );

		var cc = Local.Client.Components.Get<CustomizeComponent>();
		var skinpart = cc.GetEquippedPart( "Skins" );
		if ( skinpart != null && !string.IsNullOrEmpty( skinpart.AssetPath ) )
		{
			var skin = Material.Load( $"{skinpart.AssetPath}" );
			golfball.SetMaterialOverride( skin );
		}

		var hatpart = cc.GetEquippedPart( "Hats" );
		if ( hatpart != null && !string.IsNullOrEmpty( hatpart.AssetPath ) )
		{
			new SceneModel( SceneWorld, hatpart.AssetPath, Transform.Zero.WithScale( modelscale ).WithPosition( Vector3.Up * 2.35f * modelscale ) );
		}

		var trailpart = cc.GetEquippedPart( "Trails" );
		if ( trailpart != null && !string.IsNullOrEmpty( trailpart.AssetPath ) )
		{
			new SceneParticles( SceneWorld, trailpart.AssetPath );
		}
	}

}
