using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Gumball
{
	public class ErrorHandler
	{
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

			await ((SocketTextChannel)BotMain.botInstance.Client.GetChannel(channelId)).SendMessageAsync("", false, embedBuilder.Build());
		}
	}
}