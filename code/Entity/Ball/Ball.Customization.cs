using Facepunch.Customization;
using Sandbox;

namespace Facepunch.Minigolf.Entities;

public partial class Ball
{

    private int parthash = -1;
    private Particles trail;

    [Event.Tick.Server]
    private void EnsureCustomization()
    {
        var cc = Client.Components.GetOrCreate<CustomizeComponent>();

        var hash = cc.GetPartsHash();
        if (hash == parthash) return;

        parthash = hash;
        ApplyCustomization();
    }

    private void ApplyCustomization()
    {
        var cc = Client.Components.GetOrCreate<CustomizeComponent>();

        var trailpart = cc.GetEquippedPart("Trails");
        if (trailpart == null) return;

        trail?.Destroy();
        trail = Particles.Create(trailpart.AssetPath, this);
        trail.SetPosition(1, Vector3.One); // Color
    }

}
