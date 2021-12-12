using Newtonsoft.Json;

namespace api.Persistence
{
    public interface IIdentifiable
    {
        [JsonProperty("id")]
        string Id { get; set; }
    }
}
