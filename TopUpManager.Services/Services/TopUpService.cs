using Microsoft.Extensions.Options;
using TopUpManager.Common.Configs;
using TopUpManager.Common.Interfaces.Services;

namespace TopUpManager.Services.Services
{
    public class TopUpService : ITopUpService
    {
        private readonly Configurations _configurations;

        public TopUpService(IOptions<Configurations> config)
        {
            _configurations = config.Value;
        }

        public List<int> GetOptions()
        {
            return _configurations.TopUpOptions;
        }
    }
}
