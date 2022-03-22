
using Facepunch.Customization;
using Sandbox.UI;
using System.Linq;

namespace Facepunch.Minigolf;

[UseTemplate]
internal class CustomizePartTab : Button
{

	public CustomizationCategory Category { get; }

	private static CustomizePartTab activeTab;

	public CustomizePartTab( CustomizationCategory category )
	{
		Category = category;
	}

	protected override void OnClick( MousePanelEvent e )
	{
		base.OnClick( e );

		Open();
	}

	public void Open()
	{
		activeTab?.RemoveClass( "active" );
		AddClass( "active" );
		activeTab = this;

		Ancestors.OfType<CustomizeMenu>().FirstOrDefault()?.BuildParts( Category );
	}

}
