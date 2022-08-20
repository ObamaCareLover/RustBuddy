using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace RustBuddy.Discord
{
    class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("delaytest", "A slash command made to test the DSharpPlus Slash Commands extension!")]
        public async Task DelayTestCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            //Some time consuming task like a database call or a complex operation

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Thanks for waiting!"));

        }

        // Todo: fuck
        [SlashCommand("addentity", "Add an entity via the entities id")]
        public async Task AddEntity(InteractionContext ctx, [Option("Id","Id of the entity")]long entity = 0)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            if (entity == 0)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Please enter a valid entity id"));
                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Added entity to list"));
        }
    }
}