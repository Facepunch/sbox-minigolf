using Sandbox;

namespace Facepunch.Minigolf;

/// <summary>
/// A simple camera component that turns any entity into a camera itself.
/// </summary>
public class StaticCamera : CameraMode
{
	public override void Build( ref CameraSetup camSetup )
	{
		base.Build( ref camSetup );
		camSetup.Position = Entity.Position;
		camSetup.Rotation = Entity.Rotation;
	}

	public override void BuildInput( InputBuilder builder ) { }

    public override void Update() { }
}