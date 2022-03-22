
using Facepunch.Customization;
using Sandbox;
using Sandbox.UI;

namespace Facepunch.Minigolf;

[UseTemplate]
internal class CustomizePartIcon : Button
{

	public CustomizationPart Part { get; }

	public CustomizePartIcon( CustomizationPart part )
	{
		Part = part;
	}

	public override void Tick()
	{
		base.Tick();

		var cust = Local.Client.Components.Get<CustomizeComponent>();
		SetClass( "equipped", cust.IsEquipped( Part ) );
	}

	protected override void OnClick( MousePanelEvent e )
	{
		base.OnClick( e );

		var cust = Local.Client.Components.Get<CustomizeComponent>();
		cust.Equip( Part );
	}

}
