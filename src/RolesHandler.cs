using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gumball
{
	public class RolesHandler
	{
		public RestRole[] BotRoles
		{	
			get
			{
				return roles.Keys.ToArray();
			}
			set
			{

			}
		}

		private Dictionary<RestRole, string> roles = new Dictionary<RestRole, string>();	// Key is role, value is associated emoji

		/// <summary>
		/// Adds a role with the given color code (format AABBCC) and name.
		/// Emoji decides which emoji will be used for the reaction to get this role.
		/// </summary>
		public async Task AddRole(ulong channelId, string colorCode, string emoji, string name)
		{
			uint color = 0x000000;	// Default to black

			try { color = Convert.ToUInt32(colorCode, 16); }
			catch (FormatException)
			{
				await BotMain.botInstance.Out.PrintError(channelId, "Color should be in the hex code format: ABCDEF");
				return;
			}

			RestRole role = await BotMain.botInstance.Guild.CreateRoleAsync(name, color:color, isMentionable:false);
			roles.Add(role, emoji);

			await BotMain.botInstance.Out.PrintSuccess(channelId, $"Added role <@&{role.Id}>");
		}

		/// <summary>
		/// Removes the given role from the server.
		/// </summary>
		public async Task RemoveRole(ulong channelId, string name)
		{
			RestRole selectedRole = null;

			foreach (RestRole role in roles.Keys) if (role.Name == name) selectedRole = role;
			if (selectedRole == null)
			{
				await BotMain.botInstance.Out.PrintError(channelId, $"Role with name `{name}` does not exist");
				return;
			}

			await BotMain.botInstance.Guild.GetRole(selectedRole.Id).DeleteAsync();
			await BotMain.botInstance.Out.PrintSuccess(channelId, $"Removed role");
		}

		/// <summary>
		/// Displays the role selection embed, with reactions for each emote
		/// </summary>
		public async Task DisplayRoleMessage(string name)
		{

		}
	
		/// <summary>
		/// Listens for reactions being added, and if one is added to the reaction selection message, give the user the appropriate role
		/// </summary>
		public async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
		{
			IMessage msg = await ((SocketTextChannel)(BotMain.botInstance.Client.GetChannel(channel.Id))).GetMessageAsync(message.Id);
			Console.WriteLine(msg.Content);
		}
	}
}