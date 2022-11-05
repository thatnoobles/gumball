using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gumball
{
	public class RolesHandler
	{
		/// <summary>
		/// Represents all roles created by the bot.
		/// Key is the emoji representation for reactions, value is the actual role.
		/// </summary>
		public Dictionary<string, IRole> Roles { get; }

		private const string SAVE_FILENAME = "roles-save";

		private IUserMessage rolesMessage = null;

		public RolesHandler()
		{
			Roles = new Dictionary<string, IRole>();
		}

		/// <summary>
		/// Adds a role with the given color code (format AABBCC) and name.
		/// Emoji decides which emoji will be used for the reaction to get this role.
		/// </summary>
		public async Task AddRole(ulong channelId, string colorCode, string emoji, string name)
		{
			if (Roles.ContainsKey(emoji))
			{
				await BotMain.botInstance.Out.PrintError(channelId, $"Already created made a role with emoji {emoji}");
				return;
			}

			if (Roles.Values.Select(role => role.Name).Contains(name))
			{
				await BotMain.botInstance.Out.PrintError(channelId, $"Already created a role with name `{name}`");
				return;
			}

			uint color = 0x000000;	// Default to black

			try { color = Convert.ToUInt32(colorCode, 16); }
			catch (FormatException)
			{
				await BotMain.botInstance.Out.PrintError(channelId, "Color should be in the hex code format: ABCDEF");
				return;
			}

			IRole role = await (BotMain.botInstance.Guild.CreateRoleAsync(name, color:color, isMentionable:false));
			Roles.Add(emoji, role);

			await BotMain.botInstance.Out.PrintSuccess(channelId, $"Added role <@&{role.Id}>");

			if (rolesMessage != null)
			{
				await rolesMessage.ModifyAsync(x =>
				{
					EmbedBuilder embedBuilder = RolesMessageEmbedBuilder();
					x.Embed = embedBuilder.Build();
				});

				await rolesMessage.AddReactionAsync(new Emoji(emoji));
			}

			Save();
		}

		/// <summary>
		/// Removes the given role from the server.
		/// </summary>
		public async Task RemoveRole(ulong channelId, string name)
		{
			IRole selectedRole = null;

			foreach (IRole role in Roles.Values) if (role.Name == name) selectedRole = role;
			if (selectedRole == null)
			{
				await BotMain.botInstance.Out.PrintError(channelId, $"Role with name `{name}` does not exist");
				return;
			}

			await BotMain.botInstance.Guild.GetRole(selectedRole.Id).DeleteAsync();
			await BotMain.botInstance.Out.PrintSuccess(channelId, $"Removed role");

			foreach (string roleEmoji in Roles.Keys) if (Roles[roleEmoji].Name == selectedRole.Name)
			{
				Roles.Remove(roleEmoji);
				if (rolesMessage != null) await rolesMessage.RemoveAllReactionsForEmoteAsync(new Emoji(roleEmoji));
			}

			if (rolesMessage != null)
			{
				await rolesMessage.ModifyAsync(x =>
				{
					EmbedBuilder embedBuilder = RolesMessageEmbedBuilder();
					x.Embed = embedBuilder.Build();
				});
			}

			Save();
		}

		/// <summary>
		/// Displays the role selection embed, with reactions for each emote
		/// </summary>
		public async Task DisplayRoleMessage(ulong channelId)
		{
			EmbedBuilder embedBuilder = RolesMessageEmbedBuilder();

			if (rolesMessage != null) await BotMain.botInstance.Guild.GetTextChannel(channelId).DeleteMessageAsync(rolesMessage.Id);

			rolesMessage = await BotMain.botInstance.Guild.GetTextChannel(channelId).SendMessageAsync("", false, embedBuilder.Build());

			foreach (string emoji in Roles.Keys) await rolesMessage.AddReactionAsync(new Emoji(emoji));
			Save();
		}
	
		/// <summary>
		/// Listens for reactions being added, and if one is added to the reaction selection message, give the user the appropriate role
		/// </summary>
		public async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
		{
			IMessage msg = await BotMain.botInstance.Guild.GetTextChannel(channel.Id).GetMessageAsync(message.Id);
			
			if (reaction.User.Value.IsBot) return;
			if (msg.Id != rolesMessage.Id) return;
			if (!Roles.ContainsKey(reaction.Emote.Name)) return;

			await BotMain.botInstance.Guild.GetUser(reaction.UserId).AddRoleAsync(Roles[reaction.Emote.Name]);
		}

		/// <summary>
		/// Listens for reactions being removed, and if one is removed from the reaction selection message, remove the appropriate role from the user
		/// </summary>
		public async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
		{
			IMessage msg = await BotMain.botInstance.Guild.GetTextChannel(channel.Id).GetMessageAsync(message.Id);
			if (reaction.User.Value.IsBot) return;
			if (msg.Id != rolesMessage.Id) return;
			if (!Roles.ContainsKey(reaction.Emote.Name)) return;

			await BotMain.botInstance.Guild.GetUser(reaction.UserId).RemoveRoleAsync(Roles[reaction.Emote.Name]);
		}

		/// <summary>
		/// Caches to a file the roles message channel/message IDs, and emojis/IDs for each bot-generated role.
		/// </summary>
		public void Save()
		{
			string write = "";
			
			if (rolesMessage == null) write += $"0 0\n";
			else write += $"{rolesMessage.Channel.Id} {rolesMessage.Id}\n";

			foreach (string roleEmoji in Roles.Keys) write += $"{roleEmoji}|{Roles[roleEmoji].Id}\n";
			
			File.WriteAllText(SAVE_FILENAME, write);
		}

		/// <summary>
		/// Loads role data from a saved file, if one exists.
		/// </summary>
		public async Task Load()
		{
			if (!File.Exists(SAVE_FILENAME)) return;

			Roles.Clear();

			string[] lines = File.ReadAllLines(SAVE_FILENAME);

			for (int i = 1; i < lines.Length; i++)
			{
				string roleEmoji = lines[i].Split('|')[0];
				ulong roleId = ulong.Parse(lines[i].Split('|')[1]);

				foreach (IRole role in BotMain.botInstance.Guild.Roles)
				{
					if (role.Id == roleId)
					{
						Roles.Add(roleEmoji, role);
						break;
					}
				}
			}

			string[] roleMessageValuesRaw = lines[0].Split(' ');

			ulong channelId = ulong.Parse(roleMessageValuesRaw[0]);
			ulong messageId = ulong.Parse(roleMessageValuesRaw[1]);

			if (channelId != 0 && messageId != 0) rolesMessage = await BotMain.botInstance.Guild.GetTextChannel(channelId).ModifyMessageAsync(messageId, (x =>
			{
				EmbedBuilder embedBuilder = RolesMessageEmbedBuilder();
				x.Embed = embedBuilder.Build();
			}));

			await Program.Log(new LogMessage(LogSeverity.Info, "Gumball", $"Loaded roles [{string.Join(", ", Roles.Values.Select(role => role.Id))}]"));
		}
	
		private EmbedBuilder RolesMessageEmbedBuilder()
		{
			string description = "react with one of these emotes to get a role!\n--\n";
			foreach (string emoji in Roles.Keys) description += $"{emoji}\t<@&{Roles[emoji].Id}>\n";
			description += "--";

			EmbedBuilder embedBuilder = new EmbedBuilder()
			{
				Color = BotMain.GENERAL_EMBED_COLOR,
				Description = description,
				Footer = new EmbedFooterBuilder() { Text = "have a great day!" },
				Title = "roles!!",
			};

			return embedBuilder;
		}
	}
}