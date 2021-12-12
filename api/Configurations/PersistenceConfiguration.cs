namespace api.Configurations
{
    public class PersistenceConfiguration
    {
        public string EndPoint { get; set; }
        public string Key { get; set; }

        public string DBName { get; set; }
        public int Througput { get; set; } = 400;
    }
}
