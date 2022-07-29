using Sandbox;
using System.Collections.Generic;
using System.Linq;

using Facepunch.Minigolf.Entities;

namespace Facepunch.Minigolf;

public partial class Course : BaseNetworkable
{
    [Net] public string Name { get; set; } = "Default";
	[Net] public string Description { get; set; } = "Default Description";
	[Net] public IList<HoleInfo> Holes { get; set; }

	[Net] public int _currentHole { get; set; } = 0;
	public HoleInfo CurrentHole => Holes[_currentHole];

	public void LoadFromMap()
	{
		Host.AssertServer();
		Holes.Clear();
		
		foreach ( var hole in Entity.All.OfType<BallSpawnpoint>().OrderBy( ent => ent.HoleNumber ) )
		{
			var goal = Entity.All.OfType<HoleGoal>().Where( x => x.HoleNumber == hole.HoleNumber ).First();

			if ( goal == null )
			{
				Log.Error( $"No ball goal found for [Hole {hole.HoleNumber}]" );
				continue;
			}

			Holes.Add( new HoleInfo()
				{
					Number = hole.HoleNumber,
					Name = hole.HoleName,
					Par = hole.HolePar,
					SpawnPosition = hole.Position,
					SpawnAngles = hole.Rotation.Angles(),
					GoalPosition = goal.Position,
				}
			);
		}

		if ( Holes.Count == 0 )
		{
			Log.Error( "No holes found, is this actually a minigolf map?" );
		}
	}

	public bool IsLastHole()
	{
		return _currentHole == Holes.Count - 1;
	}

    public void NextHole()
    {
		// are we on the last hole, don't advance ( this should be checked before calling this function )
		if ( _currentHole == Holes.Count - 1 )
			return;

		_currentHole++;

		// Run an event so we can pick this up anywhere in the code base.
		Event.Run( "minigolf.advanced_hole", _currentHole );
    }
}

public partial class HoleInfo : BaseNetworkable
{
	[Net] public int Number { get; set; }
	[Net] public string Name { get; set; }
	[Net] public int Par { get; set; }
	[Net] public Vector3 SpawnPosition { get; set; }
	[Net] public Angles SpawnAngles { get; set; }
	[Net] public Vector3 GoalPosition { get; set; }
}
