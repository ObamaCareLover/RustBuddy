namespace RustBuddy.API.EventArgs
{
    public class TeamMessageEventArgs : System.EventArgs
    {
        public string Name { get; set; }
        public ulong Steam { get; set; }
        public string Message { get; set; }
        public uint Time { get; set; }
    }
}
