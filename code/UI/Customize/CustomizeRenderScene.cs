
using Facepunch.Customization;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Facepunch.Minigolf;

internal class CustomizeRenderScene : Panel
{

	private ScenePanel ScenePanel;
	private SceneWorld SceneWorld;
	private Angles CameraAngle;
	//private SceneParticles Particles;

	private int hash;

	public override void OnHotloaded()
	{
		base.OnHotloaded();

		Build();
	}

	public override void Tick()
	{
		base.Tick();

		//if ( Particles.IsValid() )
		//{
		//	Particles.Simulate( RealTime.Delta );

		//	var pos = new Vector3( -40f, MathF.Sin( Time.Now * 4f ) * 8f, -20f );
		//	Particles.SetControlPoint( 0, pos );
		//}

		var cc = Local.Client.Components.Get<CustomizeComponent>();
		if ( cc == null ) return;

		var newhash = cc.GetPartsHash();
		if ( newhash == hash ) return;

		hash = newhash;

		Build();
	}

	private void Build()
	{
		//Particles?.Delete();
		//Particles = null;

		ScenePanel?.Delete();
		ScenePanel = null;

		SceneWorld?.Delete();
		SceneWorld = new SceneWorld();

		var golfball = new SceneModel( SceneWorld, "models/golf_ball.vmdl", Transform.Zero.WithScale( 1 ) );

		ScenePanel = Add.ScenePanel( SceneWorld, Vector3.Zero, Rotation.From( CameraAngle ), 75 );
		ScenePanel.Style.Width = Length.Percent( 100 );
		ScenePanel.Style.Height = Length.Percent( 100 );
		ScenePanel.CameraPosition = golfball.Rotation.Forward * 16f + Vector3.Up * 3f;
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
			new SceneModel( SceneWorld, hatpart.AssetPath, Transform.Zero.WithScale( 1 ).WithPosition( Vector3.Up * 2.35f ) );
		}

		var trailpart = cc.GetEquippedPart( "Trails" );
		if ( trailpart != null && !string.IsNullOrEmpty( trailpart.AssetPath ) )
		{
			//Particles = new SceneParticles( SceneWorld, trailpart.AssetPath );
			//Particles.SetControlPoint( 1, Vector3.One );
		}
	}

}
