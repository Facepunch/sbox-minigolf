
using Facepunch.Customization;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Facepunch.Minigolf;

internal class CustomizeRenderScene : Panel
{

	private ScenePanel ScenePanel;
	private SceneWorld SceneWorld;
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

		new SceneLight( SceneWorld, Vector3.Up * 150.0f, 200.0f, Color.White * 5.0f );
		new SceneLight( SceneWorld, Vector3.Up * 75.0f + Vector3.Forward * 100.0f, 200, Color.White * 15.0f );
		new SceneLight( SceneWorld, Vector3.Up * 75.0f + Vector3.Backward * 100.0f, 200, Color.White * 15f );
		new SceneLight( SceneWorld, Vector3.Up * 75.0f + Vector3.Left * 100.0f, 200, Color.White * 20.0f );
		new SceneLight( SceneWorld, Vector3.Up * 75.0f + Vector3.Right * 100.0f, 200, Color.White * 15.0f );
		new SceneLight( SceneWorld, Vector3.Up * 100.0f + Vector3.Up, 200, Color.Yellow * 15.0f );

		var golfball = new SceneModel( SceneWorld, "models/golf_ball.vmdl", Transform.Zero.WithScale( 1 ) );

		ScenePanel = Add.ScenePanel( SceneWorld, Vector3.Zero, Rotation.From( CameraAngle ), 75 );
		ScenePanel.Style.Width = Length.Percent( 100 );
		ScenePanel.Style.Height = Length.Percent( 100 );
		ScenePanel.CameraPosition = golfball.Rotation.Forward * 16f + Vector3.Up * 3f;
		ScenePanel.CameraRotation = Rotation.LookAt( golfball.Rotation.Backward ).RotateAroundAxis( Vector3.Forward, -90 );
		CameraAngle = ScenePanel.CameraRotation.Angles();

		var cc = Local.Client.Components.Get<CustomizeComponent>();
		var skinpart = cc.GetEquippedPart( "Skins" );
		if ( skinpart != null && !string.IsNullOrEmpty( skinpart.AssetPath ) )
		{
			var skin = Material.Load( $"{skinpart.AssetPath}" );
			golfball.SetMaterialOverride( skin );
		}

		var hatpart = cc.GetEquippedPart( "Hats" );
		if( hatpart != null && !string.IsNullOrEmpty( hatpart.AssetPath ) )
		{
			new SceneModel( SceneWorld, hatpart.AssetPath, Transform.Zero.WithScale( 1 ).WithPosition( Vector3.Up * 2.35f ) );
		}
	}

}
