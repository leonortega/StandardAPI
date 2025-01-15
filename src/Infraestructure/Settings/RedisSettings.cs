namespace StandardAPI.Infraestructure.Settings
{
    public class RedisSettings
    {
        public string? ConnectionString { get; set; }
        public string? DefaultCacheExpiryMinutes { get; set; }
    }
}
