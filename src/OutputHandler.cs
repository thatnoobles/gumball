using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Gumball
{
	public class OutputHandler
	{
		/// <summary>
		/// Sends a formatted success message to the given channel.
		/// </summary>
		public async Task PrintSuccess(ulong channelId, string message)
		{
			EmbedBuilder embedBuilder = new EmbedBuilder()
			{
				Color = Color.Green,
				Description = $"✅ {message}"
			};

			await BotMain.botInstance.Guild.GetTextChannel(channelId).SendMessageAsync("", false, embedBuilder.Build());
		}

		/// <summary>
		/// Sends a formatted error message to the given channel.
		/// </summary>
		public async Task PrintError(ulong channelId, string message)
		{
			EmbedBuilder embedBuilder = new EmbedBuilder()
			{
				Color = Color.Red,
				Description = $"❌ {message}"
			};

			await BotMain.botInstance.Guild.GetTextChannel(channelId).SendMessageAsync("", false, embedBuilder.Build());
		}
	}
}