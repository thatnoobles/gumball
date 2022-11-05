using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Gumball
{
	public class CommandListener
	{
		/// <summary>
		/// Scans user messages for commands and perform whatever actions are necessary for each command.
		/// </summary>
		public async Task OnMessageReceived(SocketMessage message)
		{
			// COMMANDS
			// All Gumball commands start with a colon (:). User must have admin permissions to run these commands.
			//
			// ROLES
			// :role add <color-code> <emoji> <name>		Adds a new role to the server. The emoji is the reaction that will give a user the role.
			// :role remove <name>							Removes the role from the server.
			// :role message								Display the role selection message in the channel where the command was sent.
			//
			// WELCOMES
			// :welcome <add/remove>						Sets a channel to receive (or no longer receive) welcome messages

			// Cache the guild info from the first message the bot detects
			if (BotMain.botInstance.Guild == null)
			{
				BotMain.botInstance.Guild = ((SocketGuildChannel)message.Channel).Guild;
				await BotMain.botInstance.Roles.Load();
			}

			// Only listen for messages sent by humans and messages that start with :
			if (message.Author.IsBot || !message.Content.StartsWith(":")) return;

			string[] args = message.Content.Split(' ');
			if (args.Length <= 1) return;

			string command = args[0].ToLower();
			string function = args[1].ToLower();

			// :role ...
			if (command == ":role")
			{
				bool admin = await BotMain.botInstance.AdminCheck(message.Author.Id, message.Channel.Id);
				if (!admin) return;

				switch (function)
				{
					// :role add <color-code> <emoji> <name>
					case "add":
				
						try
						{
							string colorCode = $"0x{args[2].ToUpper()}";
							string emoji = args[3];
							string name = string.Empty;

							for (int i = 4; i < args.Length; i++) name += $"{args[i]} ";
							name = name.Trim();

							await BotMain.botInstance.Roles.AddRole(message.Channel.Id, colorCode, emoji, name);
						}
						// If the argument index was out of range, that means the command wasn't entered properly
						catch (IndexOutOfRangeException)
						{
							await SendRoleCommandError(message.Channel.Id);
						}

						break;

					// :role remove <name>
					case "remove":

						try
						{
							string name = string.Empty;

							for (int i = 2; i < args.Length; i++) name += $"{args[i]} ";
							name = name.Trim();

							await BotMain.botInstance.Roles.RemoveRole(message.Channel.Id, name);
						}
						// If the argument index was out of range, that means the command wasn't entered properly
						catch (IndexOutOfRangeException)
						{
							await SendRoleCommandError(message.Channel.Id);
						}

						break;

					// :role message
					case "message":
						await BotMain.botInstance.Roles.DisplayRoleMessage(message.Channel.Id);
						break;

					// Invalid function
					default:
						await SendRoleCommandError(message.Channel.Id);
						break;
				}
			}

			// :welcome <add/remove>
			else if (command == ":welcome")
			{
				bool admin = await BotMain.botInstance.AdminCheck(message.Author.Id, message.Channel.Id);
				if (!admin) return;
			}
		}

		private async Task SendRoleCommandError(ulong channelId)
		{
			await BotMain.botInstance.Out.PrintError(channelId, "**Incorrect usage, try:**\n:role add <color-code> <emoji> <name>\n:role remove <name>\n:role message");
		}
	}   
}