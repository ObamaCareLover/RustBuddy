namespace RustBuddy.API.Entities
{
    public class StorageMonitor : BaseEntity
    {
        public StorageMonitor(uint id, string name) : base(id, name)
        {
            Id = id;
            Name = name;
        }
    }
}
