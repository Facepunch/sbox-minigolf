using Sandbox;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Facepunch.Customization;

public static class Customize
{

	public static Action OnChanged;
	public static bool WatchForChanges;

	private static CustomizeConfig loadedConfig;
	private static ulong crc;
	private const string filePath = "config/customization.json";

	public static CustomizeConfig Config
	{
		get
		{
			if ( loadedConfig == null )
				loadedConfig = LoadConfig().Result;
			return loadedConfig;
		}
	}

	public static async Task<bool> IsDirty()
	{
		var newcrc = await FileSystem.Mounted.GetCRC( filePath );
		return newcrc != crc;
	}

	public static async Task<CustomizeConfig> LoadConfig()
	{
		if ( !FileSystem.Mounted.FileExists( filePath ) ) return new();

		//todo: why watcher isn't working, would make hotloading easier
		var json = FileSystem.Mounted.ReadAllText( filePath );
		loadedConfig = JsonSerializer.Deserialize<CustomizeConfig>( json );
		crc = await FileSystem.Mounted.GetCRC( filePath );

		return loadedConfig;
	}

	private static TimeSince tsdirtycheck;

	[Event.Tick]
	private static async void CheckForChange()
    {
		if ( !WatchForChanges ) return;
		if ( tsdirtycheck < 1 ) return;
		tsdirtycheck = 0;

		if( await IsDirty() )
        {
			await LoadConfig();
			OnChanged?.Invoke();
		}
	}

}
