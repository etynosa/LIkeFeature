namespace LIkeFeature.Models
{
    public class RedisConfiguration
    {
        public string ConnectionString { get; set; }
        public string Password { get; set; }
        public bool UseSentinel { get; set; }
        public List<string> Sentinels { get; set; }
        public string MasterName { get; set; }
    }
}
