namespace StandardAPI.Infraestructure.Settings
{
    public class PollySettings
    {
        public int RetryCount { get; set; }
        public TimeSpan CircuitBreakerDuration { get; set; }
        public int CircuitBreakerExceptionsAllowedBeforeBreaking { get; set; }
    }
}
