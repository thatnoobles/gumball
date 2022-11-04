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

			// All good!
			await Log(new LogMessage(LogSeverity.Info, "Gumball", "ready! :)"));

			// Don't close window automatically
			await Task.Delay(-1);
		}

		/// <summary>
		/// Sends a formatted log message to the standard out.
		/// </summary>
		public static Task Log(LogMessage msg)
		{
			Console.WriteLine(msg);
			return Task.CompletedTask;
		}
	}
}