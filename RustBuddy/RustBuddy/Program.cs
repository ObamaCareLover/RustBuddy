try
{
    var bot = new RustBuddy.Discord.Bot(
        new DSharpPlus.DiscordConfiguration
        {
            Token = "ODg4OTM1MTI1MzE0ODU5MDA5.YUZ7WQ.lYqLlmlVcYQgfMUdgu0ZsgXcSv8",
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Warning,
            TokenType = DSharpPlus.TokenType.Bot,
            AutoReconnect = true
        }
    );

    bot.Run().GetAwaiter().GetResult();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}