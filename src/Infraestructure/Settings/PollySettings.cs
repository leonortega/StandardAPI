namespace StandardAPI.Infraestructure.Settings
{
    public class PollySettings
    {
        public int RetryCount { get; set; }
        public int RetryIntervalInSeconds { get; set; }
        public int CircuitBreakerDurationInSeconds { get; set; }
        public int CircuitBreakerExceptionsAllowedBeforeBreaking { get; set; }
    }
}
