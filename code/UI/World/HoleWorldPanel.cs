using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Facepunch.Minigolf.UI;

class HoleWorldPanel : WorldPanel
{
	public HoleWorldPanel()
	{
		var w = 1040;
		var h = 760;
		PanelBounds = new Rect( -(w / 2), -(h / 2), w, h );

		StyleSheet.Load( "/UI/Styles/HoleWorldPanel.scss" );

		//Add.Label( "Hole 1", "hole" );
		//Add.Label( "Coooool", "name" );
		//Add.Label( "Par 3", "par" );
	}

	public override void Tick()
	{
		base.Tick();

		var w = 1040;
		var h = 760;
		PanelBounds = new Rect( -( w / 2 ), -( h / 2 ), w, h );
	}
}
