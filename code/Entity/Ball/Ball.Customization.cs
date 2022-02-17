using Facepunch.Customization;
using Sandbox;

namespace Facepunch.Minigolf.Entities;

public partial class Ball
{

    private int parthash = -1;
    private Particles trail;
    [Net] private AnimEntity hat { get; set; }

    private AnimEntity localhat;

    private void CleanupCustomization()
    {
        if (IsServer)
        {
            if (hat.IsValid()) hat.Delete();
            if (trail != null) trail.Destroy();
        }

        if (IsClient)
        {
            if (localhat.IsValid()) localhat.Delete();
        }

        hat = null;
        trail = null;
        localhat = null;
    }

    [Event.Tick.Server]
    private void EnsureCustomization()
    {
        var cc = Client.Components.GetOrCreate<CustomizeComponent>();

        var hash = cc.GetPartsHash();
        if (hash == parthash) return;

        parthash = hash;
        ApplyCustomization();
    }

    [Event.Tick]
    private void MoveHat()
    {
        if (!hat.IsValid()) return;

        if(IsLocalPawn && !localhat.IsValid())
        {
            hat.RenderColor = Color.Transparent;
            localhat = new AnimEntity(hat.GetModelName());
        }

        (IsLocalPawn? localhat : hat).Position = Position + Vector3.Up * 4;
    }

    private void ApplyCustomization()
    {
        var cc = Client.Components.GetOrCreate<CustomizeComponent>();

        CleanupCustomization();

        var trailpart = cc.GetEquippedPart("Trails");
        if (trailpart != null)
        {
            trail = Particles.Create(trailpart.AssetPath, this);
            trail.SetPosition(1, Vector3.One); // Color
        }

        var hatpart = cc.GetEquippedPart("Hats");
        if(hatpart != null)
        {
            hat = new AnimEntity(hatpart.AssetPath);
        }
    }

}
