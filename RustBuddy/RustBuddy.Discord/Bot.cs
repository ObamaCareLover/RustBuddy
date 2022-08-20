using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

using RustBuddy.API;
using RustBuddy.Database;

namespace RustBuddy.Discord
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }

        public Dictionary<ulong, Guild> Guilds { get; private set; } = new Dictionary<ulong, Guild>();

        public Bot(DiscordConfiguration configuration)
        {
            Client = new DiscordClient(configuration);

            Client.Ready += ClientReady;
            Client.GuildDownloadCompleted += GuildDownloadCompleted;
            Client.GuildCreated += GuildCreated;
            Client.GuildDeleted += GuildDeleted;
            Client.ComponentInteractionCreated += ComponentInteractionCreated;

            Client.UseSlashCommands().RegisterCommands<SlashCommands>(1010390891908825149);
        }

        public async Task Run()
        {
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        #region Event Handlers

        private Task ClientReady(DiscordClient c, ReadyEventArgs e)
        {
/*            if (!DataAccess.OpenConnection())
                throw new Exception("Failed to open database connection.");

            var items = DataAccess.GetItems().Result;

            if (items == null)
                return Task.CompletedTask;

            foreach (var item in items)
            {
                Items.Hashmap[item.Id] = new { Name = item.Name, Thumbnail = item.Thumbnail };
            }*/

            return Task.CompletedTask;
        }

        private Task GuildDownloadCompleted(DiscordClient c, GuildDownloadCompletedEventArgs e)
        {
            foreach (var guild in e.Guilds.Values)
            {
                if (Guilds.ContainsKey(guild.Id))
                    throw new Exception($"[GuildDownloadCompleted] Index {guild.Id} occupied.");

                Guilds.Add(guild.Id, Factory.CreateGuild(guild));
            }
            return Task.CompletedTask;
        }

        private Task GuildCreated(DiscordClient c, GuildCreateEventArgs e)
        {
            if (Guilds.ContainsKey(e.Guild.Id))
                throw new Exception($"[GuildCreated] Index {e.Guild.Id} was already occupied.");

            Guilds.Add(e.Guild.Id, Factory.CreateGuild(e.Guild));

            return Task.CompletedTask;
        }

        private Task GuildDeleted(DiscordClient c, GuildDeleteEventArgs e)
        {
            if (!Guilds.ContainsKey(e.Guild.Id))
                throw new Exception($"[GuildDeleted] Index {e.Guild.Id} does not exist.");

            Guilds.Remove(e.Guild.Id);

            return Task.CompletedTask;
        }

        private Task ComponentInteractionCreated(DiscordClient c, ComponentInteractionCreateEventArgs e)
        {
            if (!Guilds.ContainsKey(e.Interaction.Guild.Id))
                throw new Exception("[ComponentInteractionCreated] Invalid guild instance");

            if (e.Interaction.Type == InteractionType.Component)
            {
                //if (e.Interaction.Data.ComponentType == ComponentType.Button)
                //    Guilds[e.Interaction.Guild.Id].ButtonHandler(e.Interaction, e.Message);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
