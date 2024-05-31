using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using TopUpManager.Common.Configs;
using TopUpManager.Common.Entity;
using TopUpManager.Common.Exceptions;
using TopUpManager.Common.Interfaces.DataAccess;
using TopUpManager.Common.Interfaces.Services;
using TopUpManager.Common.Models.Request;

namespace TopUpManager.Services.Services
{
    public class TopUpService : ITopUpService
    {
        private readonly Configurations _configurations;
        private readonly IUserRepo _userRepo;
        private readonly IExternalFinancialService _externalFinancialService;
        private readonly ITopUpTransactionRepo _topUpTransactionRepo;
        private readonly ILogger _logger;

        public TopUpService(IOptions<Configurations> config, IUserRepo userRepo, IExternalFinancialService externalFinancialService, ITopUpTransactionRepo topUpTransactionRepo, ILogger<TopUpService> logger)
        {
            _configurations = config.Value;
            _userRepo = userRepo;
            _externalFinancialService = externalFinancialService;
            _topUpTransactionRepo = topUpTransactionRepo;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of options for top-up transactions.
        /// </summary>
        /// <returns>
        /// A list of decimal values representing the available top-up options.
        /// </returns>
        public List<decimal> GetOptions()
        {
            return _configurations.TopUpOptions;
        }


        /// <summary>
        /// Initiates a top-up transaction asynchronously.
        /// </summary>
        /// <param name="topUpRequest">The object containing information about the top-up transaction.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="topUpRequest"/> is null.</exception>
        /// <exception cref="ApiException">Thrown when various validation checks fail during the top-up process</exception>
        public async Task TopUpAsync(TopUpRequestModel topUpRequest)
        {
            if (topUpRequest == null)
            {
                _logger.LogError("topUpRequest is null");
                throw new ArgumentNullException(nameof(topUpRequest));
            }

            // Check if top-up amount is valid.
            if (!(_configurations.TopUpOptions.Contains(topUpRequest.Amount)))
            {
                _logger.LogWarning($"Invalid amount {topUpRequest.Amount}, userId: {topUpRequest.UserId}");
                throw new ApiException(HttpStatusCode.BadRequest, "Invalid amount");
            }

            // Get the fist day of current calendar month.
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Get user with all transactions of current month.
            var user = await _userRepo.GetUserWithTransactionsAsync(topUpRequest.UserId, firstDayOfMonth);

            // Check if user exists.
            if (user == null)
            {
                _logger.LogWarning($"Invalid user, userId: {topUpRequest.UserId}");
                throw new ApiException(HttpStatusCode.BadRequest, "Invalid user");
            }

            // Check if beneficiary exists for user.
            if (!(user.Beneficiaries.Any(beneficiary => beneficiary.Id == topUpRequest.BeneficiaryId)))
            {
                _logger.LogWarning($"Beneficiary: {topUpRequest.BeneficiaryId} does not exist for user, userId: {topUpRequest.UserId}");
                throw new ApiException(HttpStatusCode.BadRequest, "Invalid beneficiary");
            }

            // Check monthly limit for user
            var monthlyUserTransactionAmount = user.Beneficiaries
                                               .SelectMany(ben => ben.TopUpTransactions)
                                               .Sum(trans => trans.Amount);
            if ((topUpRequest.Amount + monthlyUserTransactionAmount) > _configurations.MonthlyLimitForUser)
            {
                _logger.LogWarning($"Monthly limit exceeded for user, userId: {topUpRequest.UserId}");
                throw new ApiException(HttpStatusCode.UnprocessableEntity, "Limit exceeded for user");
            }

            // Check monthly limit for beneficiary
            var monthlyBeneficiaryTransactionAmount = user.Beneficiaries
                                           .Where(ben => ben.Id == topUpRequest.BeneficiaryId)
                                           .SelectMany(ben => ben.TopUpTransactions)
                                           .Sum(ben => ben.Amount);
            var monthlyBeneficiaryLimit = user.IsVerified ? _configurations.VerifiedUserMonthlyBeneficiaryLimit : _configurations.UnverifiedUserMonthlyBeneficiaryLimit;

            if ((topUpRequest.Amount + monthlyBeneficiaryTransactionAmount) > monthlyBeneficiaryLimit)
            {
                _logger.LogWarning($"Monthly limit exceeded for beneficiary, userId: {topUpRequest.UserId}, beneficiary id: {topUpRequest.BeneficiaryId}");
                throw new ApiException(HttpStatusCode.UnprocessableEntity, "Limit exceeded for beneficiary");
            }

            // Get account balance from external service
            _logger.LogInformation($"Getting account balance for user id: {topUpRequest.UserId}");
            decimal balance = _externalFinancialService.GetBalance(topUpRequest.UserId);

            // Check if user has balance to do the transaction
            decimal totalTransactionAmount = topUpRequest.Amount + _configurations.TransactionCharge;
            if (balance < totalTransactionAmount)
            {
                _logger.LogWarning($"Low balance for user, userId: {topUpRequest.UserId}");
                throw new ApiException(HttpStatusCode.UnprocessableEntity, "No balance");
            }

            // Debit balance using external service
            _logger.LogInformation($"Debiting amount: {totalTransactionAmount} for userId: {topUpRequest.UserId}");
            _externalFinancialService.Debit(topUpRequest.UserId, totalTransactionAmount);
            // Add a new top up transaction for beneficiary
            var topUpTransaction = new TopUpTransaction
            {
                BeneficiaryId = topUpRequest.BeneficiaryId,
                Amount = topUpRequest.Amount,
                Date = DateTime.Now
            };
            await _topUpTransactionRepo.AddTransactionAsync(topUpTransaction);
            _logger.LogInformation($"Top up transaction successful for user id: {topUpRequest.UserId}, beneficiary id: {topUpRequest.BeneficiaryId}, amount: {topUpRequest.Amount}");
        }
    }
}
