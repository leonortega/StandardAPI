using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using StandardAPI.Infraestructure.Repositories;

namespace StandardAPI.Infraestructure.Services
{
    public class ResilientPolicyExecutor
    {
        private readonly IPolicyRegistry<string> _policyRegistry;
        private readonly ILogger<ResilientPolicyExecutor> _logger;

        public ResilientPolicyExecutor(IPolicyRegistry<string> policyRegistry, ILogger<ResilientPolicyExecutor> logger)
        {
            _policyRegistry = policyRegistry;
            _logger = logger;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, string policyKey)
        {
            _logger.LogInformation("Executing policy: {PolicyKey}", policyKey);

            try
            {
                var result = await _policyRegistry.Get<IAsyncPolicy<T>>(policyKey).ExecuteAsync(action);
                _logger.LogInformation("Policy {PolicyKey} executed successfully.", policyKey);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Policy {PolicyKey} execution failed.", policyKey);
                throw;
            }
        }

        public async Task ExecuteAsync(Func<Task> action, string policyKey)
        {
            _logger.LogInformation("Executing policy: {PolicyKey}", policyKey);

            try
            {
                await _policyRegistry.Get<IAsyncPolicy>(policyKey).ExecuteAsync(action);
                _logger.LogInformation("Policy {PolicyKey} executed successfully.", policyKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Policy {PolicyKey} execution failed.", policyKey);
                throw;
            }
        }
    }
}
