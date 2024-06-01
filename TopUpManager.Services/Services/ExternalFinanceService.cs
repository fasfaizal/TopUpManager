using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using TopUpManager.Common.Configs;
using TopUpManager.Common.Interfaces.Services;

namespace TopUpManager.Services.Services
{
    public class ExternalFinanceService : IExternalFinanceService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly Configurations _config;

        public ExternalFinanceService(ILogger<ExternalFinanceService> logger, HttpClient httpClient, IOptions<Configurations> config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _config = config.Value;
        }

        public async Task<decimal> GetBalance(int userId)
        {
            try
            {
                _logger.LogInformation($"Getting account balance for user id: {userId}");

                // Get the balance from external http service
                var response = await _httpClient.GetAsync($"{_config.FinanceServiceBaseUrl}/api/transactions/balance/{userId}");
                response.EnsureSuccessStatusCode();

                var balanceString = await response.Content.ReadAsStringAsync();
                return decimal.Parse(balanceString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching balance for user id: {userId}");
                throw;
            }
        }

        public async Task Debit(int userId, decimal amount)
        {
            try
            {
                _logger.LogInformation($"Debiting amount: {amount} for userId: {userId}");

                // Debit operation using external http service
                var debitRequestBody = new { UserId = userId, Amount = amount };
                var response = await _httpClient.PostAsJsonAsync($"{_config.FinanceServiceBaseUrl}/api/transactions/debit", debitRequestBody);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation($"{amount} debited from user : {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred. {amount} not debited from user : {userId}");
                throw;
            }
        }

        public async Task Credit(int userId, decimal amount)
        {
            try
            {
                _logger.LogInformation($"Crediting amount: {amount} for userId: {userId}");

                // Debit operation using external http service
                var debitRequestBody = new { UserId = userId, Amount = amount };
                var response = await _httpClient.PostAsJsonAsync($"{_config.FinanceServiceBaseUrl}/api/transactions/credit", debitRequestBody);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation($"{amount} credited from user : {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred. {amount} not credited from user : {userId}");
                throw;
            }
        }
    }
}
