namespace RustBuddy.API.Entities
{
    public class Alarm : BaseEntity
    {
        public string? Message { get; set; }

        public Alarm(uint id, string name) : base(id, name)
        {
            Id = id;
            Name = name;
        }
    }
}
