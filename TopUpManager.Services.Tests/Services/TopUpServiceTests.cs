using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using TopUpManager.Common.Configs;
using TopUpManager.Common.Entity;
using TopUpManager.Common.Exceptions;
using TopUpManager.Common.Interfaces.DataAccess;
using TopUpManager.Common.Interfaces.Services;
using TopUpManager.Common.Models.Request;
using TopUpManager.Services.Services;

namespace TopUpManager.Services.Tests.Services
{
    public class TopUpServiceTests
    {
        private readonly Mock<IOptions<Configurations>> _mockConfig;
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly Mock<IExternalFinancialService> _mockExternalFinancialService;
        private readonly Mock<ITopUpTransactionRepo> _mockTopUpTransactionRepo;
        private readonly Mock<ILogger<TopUpService>> _mockLogger;
        private readonly TopUpService _topUpService;

        public TopUpServiceTests()
        {
            _mockConfig = new Mock<IOptions<Configurations>>();
            _mockUserRepo = new Mock<IUserRepo>();
            _mockExternalFinancialService = new Mock<IExternalFinancialService>();
            _mockTopUpTransactionRepo = new Mock<ITopUpTransactionRepo>();
            _mockLogger = new Mock<ILogger<TopUpService>>();

            var configurations = new Configurations
            {
                TopUpOptions = new List<decimal> { 10, 20, 50, 100 },
                MonthlyLimitForUser = 1000,
                TransactionCharge = 1,
                VerifiedUserMonthlyBeneficiaryLimit = 500,
                UnverifiedUserMonthlyBeneficiaryLimit = 300
            };
            _mockConfig.Setup(config => config.Value).Returns(configurations);

            _topUpService = new TopUpService(
                _mockConfig.Object,
                _mockUserRepo.Object,
                _mockExternalFinancialService.Object,
                _mockTopUpTransactionRepo.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task TopUpAsync_ThrowsArgumentNullException_WhenTopUpRequestIsNull()
        {
            TopUpRequestModel topUpRequest = null;
            await Assert.ThrowsAsync<ArgumentNullException>(() => _topUpService.TopUpAsync(topUpRequest));
        }

        [Fact]
        public async Task TopUpAsync_ThrowsApiException_WhenAmountIsInvalid()
        {
            var topUpRequest = new TopUpRequestModel { Amount = 5, UserId = 1, BeneficiaryId = 1 };

            var exception = await Assert.ThrowsAsync<ApiException>(() => _topUpService.TopUpAsync(topUpRequest));
            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.Equal("Invalid amount", exception.Message);
        }

        [Fact]
        public async Task TopUpAsync_ThrowsApiException_WhenUserNotFound()
        {
            var topUpRequest = new TopUpRequestModel { Amount = 10, UserId = 1, BeneficiaryId = 1 };
            _mockUserRepo.Setup(repo => repo.GetUserWithTransactionsAsync(topUpRequest.UserId, It.IsAny<DateTime>())).ReturnsAsync((User)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() => _topUpService.TopUpAsync(topUpRequest));
            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.Equal("Invalid user", exception.Message);
        }

        [Fact]
        public async Task TopUpAsync_ThrowsApiException_WhenBeneficiaryNotFound()
        {
            var user = new User { Id = 1, Beneficiaries = new List<Beneficiary>() };
            var topUpRequest = new TopUpRequestModel { Amount = 10, UserId = 1, BeneficiaryId = 1 };
            _mockUserRepo.Setup(repo => repo.GetUserWithTransactionsAsync(topUpRequest.UserId, It.IsAny<DateTime>())).ReturnsAsync(user);

            var exception = await Assert.ThrowsAsync<ApiException>(() => _topUpService.TopUpAsync(topUpRequest));
            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.Equal("Invalid beneficiary", exception.Message);
        }

        [Fact]
        public async Task TopUpAsync_ThrowsApiException_WhenMonthlyLimitForUserExceeded()
        {
            var beneficiary = new Beneficiary { Id = 1, TopUpTransactions = new List<TopUpTransaction> { new TopUpTransaction { Amount = 950 } } };
            var user = new User { Id = 1, Beneficiaries = new List<Beneficiary> { beneficiary } };
            var topUpRequest = new TopUpRequestModel { Amount = 100, UserId = 1, BeneficiaryId = 1 };
            _mockUserRepo.Setup(repo => repo.GetUserWithTransactionsAsync(topUpRequest.UserId, It.IsAny<DateTime>())).ReturnsAsync(user);

            var exception = await Assert.ThrowsAsync<ApiException>(() => _topUpService.TopUpAsync(topUpRequest));
            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.StatusCode);
            Assert.Equal("Limit exceeded for user", exception.Message);
        }

        [Fact]
        public async Task TopUpAsync_ThrowsApiException_WhenMonthlyLimitForBeneficiaryExceeded()
        {
            var beneficiary = new Beneficiary { Id = 1, TopUpTransactions = new List<TopUpTransaction> { new TopUpTransaction { Amount = 290 } } };
            var user = new User { Id = 1, Beneficiaries = new List<Beneficiary> { beneficiary }, IsVerified = false };
            var topUpRequest = new TopUpRequestModel { Amount = 20, UserId = 1, BeneficiaryId = 1 };
            _mockUserRepo.Setup(repo => repo.GetUserWithTransactionsAsync(topUpRequest.UserId, It.IsAny<DateTime>())).ReturnsAsync(user);

            var exception = await Assert.ThrowsAsync<ApiException>(() => _topUpService.TopUpAsync(topUpRequest));
            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.StatusCode);
            Assert.Equal("Limit exceeded for beneficiary", exception.Message);
        }

        [Fact]
        public async Task TopUpAsync_ThrowsApiException_WhenBalanceIsInsufficient()
        {
            var beneficiary = new Beneficiary { Id = 1, TopUpTransactions = new List<TopUpTransaction>() };
            var user = new User { Id = 1, Beneficiaries = new List<Beneficiary> { beneficiary } };
            var topUpRequest = new TopUpRequestModel { Amount = 10, UserId = 1, BeneficiaryId = 1 };
            _mockUserRepo.Setup(repo => repo.GetUserWithTransactionsAsync(topUpRequest.UserId, It.IsAny<DateTime>())).ReturnsAsync(user);
            _mockExternalFinancialService.Setup(service => service.GetBalance(topUpRequest.UserId)).Returns(5);

            var exception = await Assert.ThrowsAsync<ApiException>(() => _topUpService.TopUpAsync(topUpRequest));
            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.StatusCode);
            Assert.Equal("No balance", exception.Message);
        }

        [Fact]
        public async Task TopUpAsync_SuccessfulTransaction()
        {
            var beneficiary = new Beneficiary { Id = 1, TopUpTransactions = new List<TopUpTransaction>() };
            var user = new User { Id = 1, Beneficiaries = new List<Beneficiary> { beneficiary } };
            var topUpRequest = new TopUpRequestModel { Amount = 10, UserId = 1, BeneficiaryId = 1 };
            _mockUserRepo.Setup(repo => repo.GetUserWithTransactionsAsync(topUpRequest.UserId, It.IsAny<DateTime>())).ReturnsAsync(user);
            _mockExternalFinancialService.Setup(service => service.GetBalance(topUpRequest.UserId)).Returns(100);
            _mockExternalFinancialService.Setup(service => service.Debit(topUpRequest.UserId, 11)).Verifiable();
            _mockTopUpTransactionRepo.Setup(repo => repo.AddTransactionAsync(It.IsAny<TopUpTransaction>())).Returns(Task.CompletedTask).Verifiable();

            await _topUpService.TopUpAsync(topUpRequest);

            _mockExternalFinancialService.Verify(service => service.Debit(topUpRequest.UserId, 11), Times.Once);
            _mockTopUpTransactionRepo.Verify(repo => repo.AddTransactionAsync(It.IsAny<TopUpTransaction>()), Times.Once);
        }
    }

}
