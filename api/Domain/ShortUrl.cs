using api.Persistence;

namespace api.Domain
{
    public class ShortUrl : IIdentifiable
    {
        public string Id { get; set; }
        public string Url { get; set; }

    }
}
