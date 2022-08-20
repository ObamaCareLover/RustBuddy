using DSharpPlus.Entities;

using RustBuddy.API;
using RustBuddy.Database;

namespace RustBuddy.Discord
{
    public static class Factory
    {
        public static Guild CreateGuild(DiscordGuild guild)
        {
            return new Guild(guild, new RustSocket(new RustSocketConfiguration("8.26.94.147", 27161, 76561198212256333, -1077935517)));
        }
    }
}
