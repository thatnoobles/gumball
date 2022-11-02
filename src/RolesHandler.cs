using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Gumball
{
	public class RolesHandler
	{
		/// <summary>
		/// Adds a role with the given color code (format AABBCC) and name.
		/// Emoji decides which emoji will be used for the reaction to get this role.
		/// </summary>
		public async Task AddRole(ulong channelId, string colorCode, string emoji, string name)
		{
			Console.WriteLine(colorCode);
			bool parseResult = uint.TryParse(colorCode, out uint color);

			if (!parseResult)
			{
				await BotMain.botInstance.Errors.PrintError(channelId, "Color should be in the hex code format: ABCDEF");
			}

			await BotMain.botInstance.Guild.CreateRoleAsync(name, color: new Color(color), isMentionable: true);
		}

		/// <summary>
		/// Removes the given role from the server.
		/// </summary>
		public async Task RemoveRole(string name)
		{

		}

		/// <summary>
		/// Displays the role selection embed, with reactions for each emote
		/// </summary>
		public async Task DisplayRoleMessage(string name)
		{

		}
	}
}