using Discord.WebSocket;

namespace Gumball
{
	public class BotMain
	{
		public static BotMain botInstance;	// This should bot reassigned when a new instance of the bot is created somewhere

		public DiscordSocketClient Client { get; }
		public RolesHandler Roles { get; }
		public ErrorHandler Errors { get; }

		public SocketGuild Guild { get; set; }
		public SocketTextChannel RolesChannel { get; set; }

		public BotMain()
		{
			Client = new DiscordSocketClient();
			Roles = new RolesHandler();
			Errors = new ErrorHandler();

			Client.MessageReceived += new CommandListener().OnMessageReceived;
		}
	}
}