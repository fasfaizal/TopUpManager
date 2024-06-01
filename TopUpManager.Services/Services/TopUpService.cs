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
        private readonly IExternalFinanceService _externalFinancialService;
        private readonly ITopUpTransactionRepo _topUpTransactionRepo;
        private readonly ILogger _logger;

        public TopUpService(IOptions<Configurations> config, IUserRepo userRepo, IExternalFinanceService externalFinancialService, ITopUpTransactionRepo topUpTransactionRepo, ILogger<TopUpService> logger)
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

            ValidateTopUpAmount(topUpRequest.UserId, topUpRequest.Amount);

            // Get user with all transactions of current month.
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var user = await _userRepo.GetUserWithTransactionsAsync(topUpRequest.UserId, firstDayOfMonth);

            CheckIfUserExists(topUpRequest.UserId, user);
            ValidateBeneficiaryForUser(topUpRequest.BeneficiaryId, user);
            ValidateMonthlyLimitForUser(user, topUpRequest.Amount);
            ValidateMonthlyLimitForBeneficiary(user, topUpRequest);

            // Get account balance from external service
            decimal balance = await _externalFinancialService.GetBalance(topUpRequest.UserId);
            decimal totalTransactionAmount = topUpRequest.Amount + _configurations.TransactionCharge;
            ValidateBalanceForUser(user.Id, balance, totalTransactionAmount);

            // Debit balance using external service
            await _externalFinancialService.Debit(topUpRequest.UserId, totalTransactionAmount);

            // Do the top up transaction to credit balance to beneficiary phone
            string beneficiaryPhone = user.Beneficiaries.First(ben => ben.Id == topUpRequest.BeneficiaryId).PhoneNumber;
            bool isTransactionSuccess = DoTopUp(beneficiaryPhone, topUpRequest.Amount);
            if (!isTransactionSuccess)
            {
                // If transaction fail credit money back
                await _externalFinancialService.Credit(topUpRequest.UserId, totalTransactionAmount);
                throw new ApiException(HttpStatusCode.UnprocessableEntity, "Top up transaction failed");
            }

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

        #region PrivateFunctions

        private void ValidateTopUpAmount(int userId, decimal amount)
        {
            // Check if top-up amount is valid.
            if (!(_configurations.TopUpOptions.Contains(amount)))
            {
                _logger.LogWarning($"Invalid amount {amount}, userId: {userId}");
                throw new ApiException(HttpStatusCode.BadRequest, "Invalid amount");
            }
        }

        private void CheckIfUserExists(int userId, User user)
        {
            // Check if user exists.
            if (user == null)
            {
                _logger.LogWarning($"Invalid user, userId: {userId}");
                throw new ApiException(HttpStatusCode.BadRequest, "Invalid user");
            }
        }

        private void ValidateBeneficiaryForUser(int beneficiaryId, User user)
        {
            // Check if beneficiary exists for user.
            if (user.Beneficiaries == null || !(user.Beneficiaries.Any(beneficiary => beneficiary.Id == beneficiaryId)))
            {
                _logger.LogWarning($"Beneficiary: {beneficiaryId} does not exist for user, userId: {user.Id}");
                throw new ApiException(HttpStatusCode.BadRequest, "Invalid beneficiary");
            }
        }

        private void ValidateMonthlyLimitForUser(User user, decimal amount)
        {
            // Check monthly limit for user
            var monthlyUserTransactionAmount = user.Beneficiaries
                                               .SelectMany(ben => ben.TopUpTransactions)
                                               .Sum(trans => trans.Amount);
            if ((amount + monthlyUserTransactionAmount) > _configurations.MonthlyLimitForUser)
            {
                _logger.LogWarning($"Monthly limit exceeded for user, userId: {user.Id}");
                throw new ApiException(HttpStatusCode.UnprocessableEntity, "Limit exceeded for user");
            }
        }

        private void ValidateMonthlyLimitForBeneficiary(User user, TopUpRequestModel topUpRequest)
        {
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
        }

        private void ValidateBalanceForUser(int userId, decimal balance, decimal totalTransactionAmount)
        {
            // Check if user has balance to do the transaction
            if (balance < totalTransactionAmount)
            {
                _logger.LogWarning($"Low balance for user, userId: {userId}");
                throw new ApiException(HttpStatusCode.UnprocessableEntity, "No balance");
            }
        }

        private bool DoTopUp(string number, decimal amount)
        {
            // Do the real transaction, by calling some external service if needed
            // And return true if transaction is successful or else return true
            try
            {
                _logger.LogInformation($"Starting topup transaction for {number}, amount {amount}");
                return true;
            }
            catch
            {
                _logger.LogError($"Topup transaction failed for {number} for amount {amount}");
                return false;
            }
        }

        #endregion
    }
}
