using Discord.WebSocket;

namespace Gumball
{
	public class BotMain
	{
		public static BotMain botInstance;	// This should bot reassigned when a new instance of the bot is created somewhere

		public DiscordSocketClient Client { get; }
		public RolesHandler Roles { get; }
		public OutputHandler Out { get; }

		public SocketGuild Guild { get; set; }
		public SocketTextChannel RolesChannel { get; set; }

		public BotMain()
		{
			DiscordSocketConfig config = new DiscordSocketConfig() { MessageCacheSize = 10 };

			Client = new DiscordSocketClient(config);
			Roles = new RolesHandler();
			Out = new OutputHandler();

			Client.MessageReceived += new CommandListener().OnMessageReceived;
			Client.ReactionAdded += Roles.OnReactionAdded;
		}
	}
}