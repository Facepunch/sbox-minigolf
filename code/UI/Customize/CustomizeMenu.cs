using Facepunch.Customization;
using Sandbox;
using Sandbox.UI;
using System.Linq;

namespace Facepunch.Minigolf;

[UseTemplate]
public class CustomizeMenu : Panel
{

    public Panel SidebarCanvas { get; set; }
    public Panel ContentCanvas { get; set; }

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
        get => HasClass("open");
        set => SetClass("open", value);
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

    private Button activeButton;
    private void Build()
    {
        SidebarCanvas?.DeleteChildren(true);
        ContentCanvas?.DeleteChildren(true);

        var categories = Customization.Customize.Config.Categories;
        foreach(var cat in categories)
        {
            var btn = new Button(cat.DisplayName);
            SidebarCanvas.AddChild(btn);

            btn.AddEventListener("onmousedown", () =>
            {
                activeButton?.RemoveClass("active");
                btn.AddClass("active");
                activeButton = btn;

                LoadParts(cat);
            });
        }
    }

    private void LoadParts(CustomizationCategory category)
    {
        ContentCanvas?.DeleteChildren(true);

        var cfg = Customization.Customize.Config;
        var parts = cfg.Parts.Where(x => x.CategoryId == category.Id);

        foreach(var part in parts)
        {
            var btn = new Button(part.DisplayName);
            ContentCanvas.AddChild(btn);

            var cust = Local.Client.Components.Get<CustomizeComponent>();
            btn.SetClass("equipped", cust.IsEquipped(part));

            btn.AddEventListener("onmousedown", () =>
            {
                cust.Equip(part);
                LoadParts(category);
            });
        }
    }

    [Event.BuildInput]
    private void OnBuildInput(InputBuilder b)
    {
        if(b.Pressed(InputButton.Menu))
        {
            Open = !Open;
        }
    }

}
