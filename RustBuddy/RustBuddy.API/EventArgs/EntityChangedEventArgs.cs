namespace RustBuddy.API.EventArgs
{
    public class EntityChangedEventArgs : System.EventArgs
    {
        public uint Id { get; set; }
        public bool State { get; set; }
    }
}
