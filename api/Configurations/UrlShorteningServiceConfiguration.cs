namespace api.Configurations
{
    public class UrlShorteningServiceConfiguration
    {
        public string ServerBaseUrl { get; set; }
        public string SequenceName { get; set; }
        public ulong ChunkSize { get; set; } = 100;
    }
}
