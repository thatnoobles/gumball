using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Gumball
{
	public class BotMain
	{
		public static BotMain botInstance;	// This should be reassigned when a new instance of the bot is created somewhere

		public const uint GENERAL_EMBED_COLOR = 0x5cb1db;

		public DiscordSocketClient Client { get; }
		public RolesHandler Roles { get; }
		public WelcomesHandler Welcomes { get; }
		public OutputHandler Out { get; }

		public SocketGuild Guild { get; set; }
		public SocketTextChannel RolesChannel { get; set; }

		public BotMain()
		{
			DiscordSocketConfig config = new DiscordSocketConfig()
			{
				// GuildMembers intent needs to be enabled so we can listen for users joining
				GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
				MessageCacheSize = 100
			};

			Client = new DiscordSocketClient(config);
			Roles = new RolesHandler();
			Welcomes = new WelcomesHandler();
			Out = new OutputHandler();

			Client.MessageReceived += new CommandListener().OnMessageReceived;
			Client.ReactionAdded += Roles.OnReactionAdded;
			Client.ReactionRemoved += Roles.OnReactionRemoved;
			Client.UserJoined += Welcomes.OnUserJoined;
		}
	
		/// <summary>
		/// Checks to see if the given user is an admin. Returns true if yes, returns false and prints an error message if not.
		/// </summary>
		public async Task<bool> AdminCheck(ulong userId, ulong channelId)
		{
			bool admin = botInstance.Guild.GetUser(userId).GuildPermissions.Administrator;

			if (!admin) await Out.PrintError(channelId, "You must be an administrator to do this");
			return admin;
		}
	}
}