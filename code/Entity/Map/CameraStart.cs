namespace Facepunch.Minigolf.Entities;

[Library( "minigolf_hole_camera")]
[HammerEntity, EditorModel( "models/editor/camera.vmdl" )]
[Title( "Hole Camera" )]
public partial class HoleCamera : Entity
{
	[Property( "hole_number", "Hole Number", "Which hole this camera is for" ), Net]
	public int Hole { get; set; } = 1;

	[Property( "FOV", "Field of view in degrees" ), Net]
	public float FOV { get; set; } = 90.0f;
	[Property( "ZNear", "Distance to the near plane" ), Net]
	public float ZNear { get; set; } = 4.0f;
	[Property( "ZFar", "Distance to the far plane" ), Net]
	public float ZFar { get; set; } = 10000.0f;

	public override void Spawn()
	{
		Transmit = TransmitType.Always;
	}

	public override void ClientSpawn()
	{
		var camera = Components.Create<StaticCamera>();
		camera.FieldOfView = FOV;
		camera.ZNear = ZNear;
		camera.ZFar = ZFar;
	}
}

[Library( "minigolf_start_camera" )]
[HammerEntity, EditorModel( "models/editor/camera.vmdl" ), FrustumBoundless( "FOV", "ZNear", "ZFar" )]
[Title( "Start Camera" )]
public partial class StartCamera : Entity
{
	[Property( "FOV", "Field of view" ), Net]
	public float FOV { get; set; } = 90.0f;
	[Property( "ZNear", "Distance to the near plane" ), Net]
	public float ZNear { get; set; } = 4.0f;
	[Property( "ZFar", "Distance to the far plane" ), Net]
	public float ZFar { get; set; } = 10000.0f;

	public override void Spawn()
	{
		Transmit = TransmitType.Always;
	}

	public override void ClientSpawn()
	{
		var camera = Components.Create<StaticCamera>();
		camera.FieldOfView = FOV;
		camera.ZNear = ZNear;
		camera.ZFar = ZFar;
	}
}
