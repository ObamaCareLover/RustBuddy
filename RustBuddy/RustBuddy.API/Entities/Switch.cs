namespace RustBuddy.API.Entities
{
    public class Switch : BaseEntity
    {
        public bool State { get; set; } = false;

        public Switch(uint id, string name) : base(id, name)
        {
            Id = id;
            Name = name;
        }

        public async Task<bool> SetState(IRustSocket Socket, bool state)
        {
            if (await Socket.SetEntityState(Id, state))
                return true;
            return false;
        }
    }
}
