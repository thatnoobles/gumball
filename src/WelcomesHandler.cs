using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Gumball
{
	public class WelcomesHandler
	{
		private const string SAVE_FILENAME = "welcome-save";

		private IChannel welcomesChannel = null;

		/// <summary>
		/// Sets the given channel as the one to send welcome embeds.
		/// </summary>
		public async Task Set(ulong channelId)
		{
			welcomesChannel = BotMain.botInstance.Guild.GetTextChannel(channelId);
			await BotMain.botInstance.Out.PrintSuccess(channelId, "Set channel to send welcome embeds");

			Save();
		}

		/// <summary>
		/// Sets the given channel as no longer sending welcome embeds, if it already does.
		/// </summary>
		public async Task Remove(ulong channelId)
		{
			if (welcomesChannel == null || welcomesChannel.Id != channelId)
			{
				await BotMain.botInstance.Out.PrintError(channelId, "This channel doesn't send welcome embeds");
				return;
			}

			File.Delete(SAVE_FILENAME);
			await BotMain.botInstance.Out.PrintSuccess(channelId, "This channel will no longer send welcome embeds");
		}

		/// <summary>
		/// When user joins the server, display a nice little embed.
		/// </summary>
		public async Task OnUserJoined(SocketGuildUser user)
		{
			Console.WriteLine(user.DisplayName);

			if (welcomesChannel == null) return;

			EmbedBuilder embedBuilder = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { IconUrl = user.GetAvatarUrl(), Name = user.DisplayName },
				Color = BotMain.GENERAL_EMBED_COLOR,
				Description = "welcome to meow meow! please check out <#1036039443292893275> and all the channels listed under member info.",
				Footer = new EmbedFooterBuilder() { Text = "i'm happy to have you here!" },
			};

			await BotMain.botInstance.Guild.GetTextChannel(welcomesChannel.Id).SendMessageAsync($"hi <@{user.Id}>!", false, embedBuilder.Build());
		}

		/// <summary>
		/// Writes welcome channel ID to a file.
		/// </summary>
		public void Save() => File.WriteAllText(SAVE_FILENAME, welcomesChannel.Id.ToString());

		/// <summary>
		/// Loads welcome channel ID from the saved file.
		/// </summary>
		public void Load()
		{
			if (!File.Exists(SAVE_FILENAME)) return;
			welcomesChannel = BotMain.botInstance.Guild.GetTextChannel(ulong.Parse(File.ReadAllLines(SAVE_FILENAME)[0]));
		}
	}
}