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
            _logger.LogInformation($"Getting account balance for user id: {userId}");
            return 100;
        }

        public void Debit(int userId, decimal amount)
        {
            // Call the external service API and do the debit operation.
            // Can implement a retry logic also in case of failure
            try
            {
                _logger.LogInformation($"Debiting amount: {amount} for userId: {userId}");
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

        public void Credit(int userId, decimal amount)
        {
            // Call the external service API and do the credit operation.
            // Can implement a retry logic also in case of failure
            try
            {
                _logger.LogInformation($"Crediting amount: {amount} for userId: {userId}");
                // Credit operation 
                _logger.LogInformation($"{amount} credited from user : {userId}");

                // If not success throw error
            }
            catch
            {
                _logger.LogError($"Error occurred. {amount} not credited from user : {userId}");
                throw;
            }
        }
    }
}
