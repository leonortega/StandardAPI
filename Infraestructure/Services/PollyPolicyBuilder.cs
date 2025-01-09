using Polly;
using Polly.Registry;

namespace StandardAPI.Infraestructure.Services
{
    public class ResilientPolicyExecutor
    {
        private readonly IPolicyRegistry<string> _policyRegistry;

        public ResilientPolicyExecutor(IPolicyRegistry<string> policyRegistry)
        {
            _policyRegistry = policyRegistry;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, string policyKey)
        {
            if (!_policyRegistry.TryGet<IAsyncPolicy<T>>(policyKey, out var policy))
            {
                throw new InvalidOperationException($"Policy with key '{policyKey}' not found.");
            }

            return await policy.ExecuteAsync(action);
        }

        public async Task ExecuteAsync(Func<Task> action, string policyKey)
        {
            if (!_policyRegistry.TryGet<IAsyncPolicy>(policyKey, out var policy))
            {
                throw new InvalidOperationException($"Policy with key '{policyKey}' not found.");
            }

            await policy.ExecuteAsync(action);
        }
    }
}
