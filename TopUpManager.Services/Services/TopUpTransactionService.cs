using Microsoft.Extensions.Options;
using TopUpManager.Common.Configs;
using TopUpManager.Common.Interfaces.Services;

namespace TopUpManager.Services.Services
{
    public class TopUpTransactionService : ITopUpTransactionService
    {
        private readonly Configurations _configurations;

        public TopUpTransactionService(IOptions<Configurations> config)
        {
            _configurations = config.Value;
        }

        public List<int> GetOptions()
        {
            return _configurations.TopUpOptions;
        }
    }
}
