using Microsoft.Extensions.Logging;
using TopUpManager.Common.Interfaces.Services;

namespace TopUpManager.Services.Services
{
    public class ExternalFinancialService : IExternalFinancialService
    {
        private readonly ILogger _logger;

        public ExternalFinancialService(ILogger<ExternalFinancialService> logger)
        {
            _logger = logger;
        }

        public decimal GetBalance(int userId)
        {
            // Call the external service API to get the users balance.
            return 100;
        }

        public void Debit(int userId, decimal amount)
        {
            // Call the external service API and do the debit operation.
            // Can implement a retry logic also in case of failure
            try
            {
                // Debit operation 
                _logger.LogInformation($"{amount} debited from user : {userId}");

                // If not success throw error
            }
            catch
            {
                _logger.LogError($"Error occurred. {amount} not debited from user : {userId}");
                throw;
            }
        }
    }
}
