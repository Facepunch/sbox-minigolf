using Facepunch.Customization;
using Sandbox;
using Sandbox.UI;
using System.Linq;

namespace Facepunch.Minigolf;

[UseTemplate]
public class CustomizeMenu : Panel
{

	public Panel TabsCanvas { get; set; }
	public Panel PartsCanvas { get; set; }

	public CustomizeMenu()
	{
		Customize.OnChanged += Build;
	}

	public override void OnDeleted()
	{
		base.OnDeleted();

		Customize.OnChanged -= Build;
	}

	private bool Open
	{
		get => HasClass( "open" );
		set => SetClass( "open", value );
	}

	protected override void PostTemplateApplied()
	{
		base.PostTemplateApplied();

		Build();
	}

	public override void OnHotloaded()
	{
		base.OnHotloaded();

		Build();
	}

	private void Build()
	{
		TabsCanvas?.DeleteChildren( true );
		PartsCanvas?.DeleteChildren( true );

		var categories = Customize.Config.Categories;
		var oneopen = false;
		foreach ( var cat in categories )
		{
			var btn = new CustomizePartTab( cat );
			TabsCanvas.AddChild( btn );

			if ( !oneopen )
			{
				btn.Open();
				oneopen = true;
			}
		}
	}

	public void BuildParts( CustomizationCategory category )
	{
		PartsCanvas?.DeleteChildren( true );

		var cfg = Customization.Customize.Config;
		var parts = cfg.Parts.Where( x => x.CategoryId == category.Id );

		foreach ( var part in parts )
		{
			var btn = new CustomizePartIcon( part );
			PartsCanvas.AddChild( btn );
		}
	}

	[Event.BuildInput]
	private void OnBuildInput( InputBuilder b )
	{
		if ( b.Pressed( InputButton.Menu ) )
		{
			Open = !Open;
		}
	}

}
