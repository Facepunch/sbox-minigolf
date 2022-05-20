using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Facepunch.Minigolf.UI;

public partial class ChatBox : Panel
{
	static ChatBox Current;
	public Panel Canvas { get; protected set; }

	public TextEntry Input { get; protected set; }
	int msgCount = 0;
	public ChatBox()
	{
		Current = this;
		StyleSheet.Load( "/UI/ChatBox/ChatBox.scss" );
		Canvas = Add.Panel( "chat_canvas" );
		Canvas.PreferScrollToBottom = true;

		Input = Add.TextEntry( "" );
		Input.AddEventListener( "onsubmit", () => Submit() );
		Input.AddEventListener( "onblur", () => Close() );
		Input.AcceptsFocus = true;
		Input.AllowEmojiReplace = true;

		Sandbox.Hooks.Chat.OnOpenChat += Open;
	}

	public override void Tick()
	{
		//Input.CaretColor = new Color( 0.1714f, 0.831f, 0.5292f );

		base.Tick();
	}
	void Open()
	{
		AddClass( "open" );
		Input.Focus();
	}

	void Close()
	{
		RemoveClass( "open" );
		Input.Blur();
	}

	void Submit()
	{
		Close();

		var msg = Input.Text.Trim();
		Input.Text = "";

		if ( string.IsNullOrWhiteSpace( msg ) )
			return;

		Say( msg );
	}

	public void AddEntry( string name, string message, string avatar )
	{
		msgCount++;
		var e = Canvas.AddChild<ChatEntry>();
		//e.SetFirstSibling();
		e.Message.Text = message;
		e.NameLabel.Text = name;
		e.Avatar.SetTexture( avatar );

		e.SetClass( "noname", string.IsNullOrEmpty( name ) );
		e.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );
		ScrollToBottom();
	}

	// TODO: Maybe needs to be networked or check for other peoples messages 
	private async void ScrollToBottom()
	{
		if ( !Canvas.IsScrollAtBottom )
		{
			Canvas.TryScroll( 5 );
			await Task.Delay( 100 );
			ScrollToBottom();

		}
		return;
	}

	[ConCmd.Client( "chat_add", CanBeCalledFromServer = true )]
	public static void AddChatEntry( string name, string message, string avatar = null )
	{
		Current?.AddEntry( name, message, avatar );

		// Only log clientside if we're not the listen server host
		if ( !Global.IsListenServer )
		{
			Log.Info( $"{name}: {message}" );
		}
	}

	[ConCmd.Client( "chat_addinfo", CanBeCalledFromServer = true )]
	public static void AddInformation( string message, string avatar = null )
	{
		Current?.AddEntry( null, message, avatar );
	}

	[ConCmd.Client( "chat_twitch", CanBeCalledFromServer = true )]
	public static void AddTwitch( string user, string message )
	{
		Current?.AddEntry( user, message, "ui/twitch.jpg" );
	}

	[ConCmd.Server( "say" )]
	public static void Say( string message )
	{
		Assert.NotNull( ConsoleSystem.Caller );

		// todo - reject more stuff
		if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
			return;

		Log.Info( $"{ConsoleSystem.Caller}: {message}" );
		AddChatEntry( To.Everyone, $"{ConsoleSystem.Caller.Name}", message, $"avatar:{ConsoleSystem.Caller.PlayerId}" );
	}
}

