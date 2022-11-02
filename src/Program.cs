// made for ava, with love <3

using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Gumball
{  
	internal class Program
	{
		private static Task Main() => new Program().MainAsync();

		private async Task MainAsync()
		{
			BotMain.botInstance = new BotMain();

			// Setup background processes (logging, listening for commands, etc)
			BotMain.botInstance.Client.Log += Log;

			// Log the bot in using its auth token and start it up
			await BotMain.botInstance.Client.LoginAsync(TokenType.Bot, File.ReadAllText("token.txt"));
			await BotMain.botInstance.Client.StartAsync();

			// Don't close window automatically
			await Task.Delay(-1);
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg);
			return Task.CompletedTask;
		}
	}
}