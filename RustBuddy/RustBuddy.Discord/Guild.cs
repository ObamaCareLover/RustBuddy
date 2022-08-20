using RustBuddy.API;
using RustBuddy.API.Entities;
using RustBuddy.API.EventArgs;
using RustBuddy.Database;

using DSharpPlus.Entities;

namespace RustBuddy.Discord
{
    public class Guild
    {
        private readonly DiscordGuild _ctx;
        private readonly IRustSocket _socket;

        private List<BaseEntity> Entities = new();

        public Guild(DiscordGuild guild, IRustSocket socket)
        {
            _ctx = guild;
            _socket = socket;
            
            _socket.OnEntityChanged += OnEntityChanged;
            _socket.OnTeamMessage += OnTeamMessage;

            //Debug shit
            AddSwitch(21513300, "Smart Switch");
        }

        public async void AddSwitch(uint entity_id, string name, bool subscribe = true)
        {
            var entity = await _socket.GetEntityInfo(entity_id, subscribe);

            if (entity == null)
                return;

            switch (entity.Type)
            {
                case AppEntityType.Switch:
                    Entities.Add(new Switch(entity_id, name) { Subscribed = true });
                    Console.WriteLine($"Adding new entity of type {typeof(Switch).Name}");

                    break;

                case AppEntityType.Alarm:
                    Entities.Add(new Alarm(entity_id, name) { Subscribed = true });
                    Console.WriteLine($"Adding new entity of type {typeof(Alarm).Name}");

                    break;

                case AppEntityType.StorageMonitor:
                    Entities.Add(new StorageMonitor(entity_id, name) { Subscribed = true });
                    Console.WriteLine($"Adding new entity of type {typeof(StorageMonitor).Name}");

                    break;

                default:
                    throw new Exception("Unknown entity type");
            }
        }

        #region Event Handlers

        protected async void OnEntityChanged(object? sender, EntityChangedEventArgs e)
        {
            var entity = Entities.FirstOrDefault(value => value.Id == e.Id);

            if (entity == null)
                return;

            switch (entity)
            {
                case Switch sw:
                    sw.State = e.State;

                    break;

                case Alarm a:
                    if (e.State != true)
                        return;

                    break;

                case StorageMonitor s:
                    if (e.State != true)
                        return;

                    // Update a message with current storage loot?
                    // e.Broadcast.EntityChanged.Payload.Items.ToList()

                    break;
            }
        }

        protected async void OnTeamMessage(object? sender, TeamMessageEventArgs e)
        {
            if (e.Message.StartsWith(".lockdown"))
            {
                foreach (var entity in Entities.OfType<Switch>().Where(value => value.State == false))
                    await entity.SetState(_socket, true);
            }
            else if (e.Message.StartsWith(".forceoff"))
            {
                foreach (var entity in Entities.OfType<Switch>().Where(value => value.State == true))
                    await entity.SetState(_socket, false);
            }
            else if (e.Message.StartsWith(".time"))
            {
                var time = await _socket.GetTime();

                var str = "Current time: " + Math.Round(time.Time, 2, MidpointRounding.ToZero).ToString().Replace(".", ":");

                if (time.Time < time.Sunset)
                    str += $" Sunset: " + Math.Round(time.Sunset, 2, MidpointRounding.ToZero).ToString().Replace(".", ":");

                else
                    str += $" Sunrise: " + Math.Round(time.Sunrise, 2, MidpointRounding.ToZero).ToString().Replace(".", ":");

                await _socket.SendTeamMessage(str);
            }
        }

        #endregion

        #region Interaction Handlers

        //public async void ButtonHandler(DiscordInteraction interaction, DiscordMessage message)
        //{
        //    await interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);

        //    var smsg = Messages.FirstOrDefault(value => value.Message.Id == message.Id);

        //    if (smsg == null)
        //        throw new Exception("[ButtonHandler] Component not in tracked message list");

        //    if (interaction.Data.CustomId == "off")
        //    {
        //        if (await Socket.SetEntityState(smsg.Entity.Id, false))
        //            smsg.Entity.State = false;
        //        else
        //            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
        //                new DiscordInteractionResponseBuilder().WithContent("Failed to set entity state"));

        //    }
        //    else
        //    {
        //        if (await Socket.SetEntityState(smsg.Entity.Id, true))
        //            smsg.Entity.State = true;
        //        else
        //            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
        //                new DiscordInteractionResponseBuilder().WithContent("Failed to set entity state"));
        //    }
        //    await smsg.Message.ModifyAsync(Factory.CreateDiscordMessageBuilder(smsg.Entity));
        //}

        #endregion
    }
}
