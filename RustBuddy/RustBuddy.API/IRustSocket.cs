namespace RustBuddy.API
{
    public interface IRustSocket
    {
        public event EventHandler<EventArgs.EntityChangedEventArgs> OnEntityChanged;
        public event EventHandler<EventArgs.TeamMessageEventArgs> OnTeamMessage;
        public event EventHandler<EventArgs.TeamChangedEventArgs> OnTeamChanged;

        public Task<bool> SendTeamMessage(string message);

        public Task<bool> SetEntityState(uint entity_id, bool state);

        public Task<AppEntityInfo> GetEntityInfo(uint entity_id, bool subscribe = true);

        public Task<IList<AppMarker>> GetMapMarkers();

        public Task<AppTeamChat> GetTeamChat();

        public Task<AppTime> GetTime();
    }
}
