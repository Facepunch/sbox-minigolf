
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
	private SceneModel GolfBall;
	private Angles CameraAngle;

	private int hash;

	public CustomizeRenderScene()
	{
		Build();
	}

	public override void OnHotloaded()
	{
		base.OnHotloaded();

		Build();
	}

	public override void Tick()
	{
		base.Tick();

		var cc = Local.Client.Components.Get<CustomizeComponent>();
		var skinpart = cc.GetEquippedPart( "Skins" );

		// todo: hash, apply for all parts 
		if ( skinpart == null ) return;

		var newhash = skinpart.GetHashCode();
		if ( hash == newhash ) return;

		hash = newhash;

		var skin = Material.Load( $"{skinpart.AssetPath}" );
		GolfBall.SetMaterialOverride( skin );

		Log.Info( "SET" );
	}

	private void Build()
	{
		ScenePanel?.Delete();
		ScenePanel = null;

		SceneWorld?.Delete();
		SceneWorld = new SceneWorld();

		GolfBall = new SceneModel( SceneWorld, "models/golf_ball.vmdl", Transform.Zero.WithScale( 1 ) );

		new SceneLight( SceneWorld, Vector3.Up * 150.0f, 200.0f, Color.White * 5.0f );
		new SceneLight( SceneWorld, Vector3.Up * 75.0f + Vector3.Forward * 100.0f, 200, Color.White * 15.0f );
		new SceneLight( SceneWorld, Vector3.Up * 75.0f + Vector3.Backward * 100.0f, 200, Color.White * 15f );
		new SceneLight( SceneWorld, Vector3.Up * 75.0f + Vector3.Left * 100.0f, 200, Color.White * 20.0f );
		new SceneLight( SceneWorld, Vector3.Up * 75.0f + Vector3.Right * 100.0f, 200, Color.White * 15.0f );
		new SceneLight( SceneWorld, Vector3.Up * 100.0f + Vector3.Up, 200, Color.Yellow * 15.0f );

		ScenePanel = Add.ScenePanel( SceneWorld, Vector3.Zero, Rotation.From( CameraAngle ), 75 );
		ScenePanel.Style.Width = Length.Percent( 100 );
		ScenePanel.Style.Height = Length.Percent( 100 );
		ScenePanel.CameraPosition = GolfBall.Rotation.Forward * 10f;
		ScenePanel.CameraRotation = Rotation.LookAt( GolfBall.Rotation.Backward ).RotateAroundAxis( Vector3.Forward, -90 );
		//ScenePanel.CameraRotation = Rotation.From( 10, -62, 0 );
		CameraAngle = ScenePanel.CameraRotation.Angles();
	}

}
