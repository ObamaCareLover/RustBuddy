namespace RustBuddy.API.Entities
{
    public abstract class BaseEntity
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public bool Subscribed { get; set; } = false;

        public BaseEntity(uint id, string name)
        {
            Id = id;
            Name = name;
        }

        public async Task<bool> IsAlive(IRustSocket Socket)
        {
            var exist = await Socket.GetEntityInfo(Id, false);

            return exist != null;
        }
    }
}
