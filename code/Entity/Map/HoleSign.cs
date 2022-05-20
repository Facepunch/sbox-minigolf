using Facepunch.Minigolf.UI;

namespace Facepunch.Minigolf.Entities;

[Library( "minigolf_hole_sign" )]
[HammerEntity, DrawAngles, EditorSprite( "materials/editor/hole_sign/hole_sign.vmat" )]
[Title( "Hole Sign" )]
public partial class HoleSign : Entity
{
	HoleWorldPanel WorldPanel;

	[Net]
	[Property( "Hole", Title = "Hole" )]
	public string Hole { get; set; }

	[Net]
	[Property( "Par", Title = "Par" )]
	public string Par { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}
	public override void ClientSpawn()
	{
		base.ClientSpawn();

		WorldPanel = new();
		WorldPanel.Transform = Transform;

		WorldPanel.Style.Opacity = 1;

		var ttext = Hole;
		var btext = Par;


		// Bring it out the smallest amount from the sign
		WorldPanel.Transform = WorldPanel.Transform.WithPosition( WorldPanel.Transform.Position + WorldPanel.Transform.Rotation.Forward * 0.05f );

		WorldPanel.Add.Label( $"Hole {ttext}", "hole" );
		WorldPanel.Add.Label( $"_________", "name" );
		WorldPanel.Add.Label( $"Par {btext}", "par" );

	}

	[ClientRpc, Input]
	public void DisplayText()
	{
		WorldPanel.Style.Opacity = 1;
	}

	[ClientRpc, Input]
	public void HideText()
	{
		WorldPanel.Style.Opacity = 0;
	}
}
