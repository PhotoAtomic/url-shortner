using api.Persistence;

namespace api.Domain
{
    public class Sequence : IIdentifiable
    {
        public string Id { get; set; }        

        public ulong Limit;
    }
}
