using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using TopUpManager.Common.Configs;
using TopUpManager.Common.Entity;
using TopUpManager.Common.Exceptions;
using TopUpManager.Common.Interfaces.DataAccess;
using TopUpManager.Common.Models.Request;
using TopUpManager.Services.Services;

namespace TopUpManager.Services.Tests.Services
{
    public class BeneficiaryServiceTests
    {
        private readonly Mock<IBeneficiaryRepo> _mockBeneficiaryRepo;
        private readonly BeneficiaryService _beneficiaryService;
        private readonly Mock<IOptions<Configurations>> _configMock;
        private readonly Mock<IUserRepo> _mockUserRepo;

        public BeneficiaryServiceTests()
        {
            _mockBeneficiaryRepo = new Mock<IBeneficiaryRepo>();
            _mockUserRepo = new Mock<IUserRepo>();
            _configMock = new Mock<IOptions<Configurations>>();
            _configMock.Setup(c => c.Value).Returns(new Configurations
            {
                MaxBeneficiaryCount = 1,
            });
            _beneficiaryService = new BeneficiaryService(_mockBeneficiaryRepo.Object, _mockUserRepo.Object, _configMock.Object);
        }

        [Fact]
        public async Task GetBeneficiariesAsync_ReturnsListOfBeneficiaries()
        {
            int userId = 1;
            var expectedBeneficiaries = new List<Beneficiary>
            {
                new Beneficiary { Id = 1, Nickname = "Beneficiary 1", UserId = userId },
                new Beneficiary { Id = 2, Nickname = "Beneficiary 2" ,UserId = userId },
            };
            _mockBeneficiaryRepo.Setup(repo => repo.GetBeneficiariesAsync(userId))
                               .ReturnsAsync(expectedBeneficiaries);

            var result = await _beneficiaryService.GetBeneficiariesAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(expectedBeneficiaries, result);
        }

        [Fact]
        public async Task GetBeneficiariesAsync_ReturnsEmptyList_WhenNoBeneficiariesFound()
        {
            int userId = 1;
            _mockBeneficiaryRepo.Setup(repo => repo.GetBeneficiariesAsync(userId))
                               .ReturnsAsync(new List<Beneficiary>());

            var result = await _beneficiaryService.GetBeneficiariesAsync(userId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateBeneficiaryAsync_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _beneficiaryService.CreateBeneficiaryAsync(null));
        }

        [Fact]
        public async Task CreateBeneficiaryAsync_InvalidUser_ThrowsApiException()
        {
            var beneficiaryRequest = new BeneficiaryRequestModel { Nickname = "Nickname", UserId = 1 };

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(beneficiaryRequest.UserId))
                         .ReturnsAsync((User)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() => _beneficiaryService.CreateBeneficiaryAsync(beneficiaryRequest));
            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.Equal("Invalid user", exception.Message);
        }

        [Fact]
        public async Task CreateBeneficiaryAsync_ExceedsMaxBeneficiaryCount_ThrowsApiException()
        {
            var beneficiaryRequest = new BeneficiaryRequestModel { Nickname = "Nickname", UserId = 1 };
            var user = new User { Id = 1, Beneficiaries = new List<Beneficiary> { new Beneficiary() } };

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(beneficiaryRequest.UserId))
                         .ReturnsAsync(user);

            var exception = await Assert.ThrowsAsync<ApiException>(() => _beneficiaryService.CreateBeneficiaryAsync(beneficiaryRequest));
            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.StatusCode);
            Assert.Equal("Beneficiary limit reached", exception.Message);
        }

        [Fact]
        public async Task CreateBeneficiaryAsync_ValidRequest_ReturnsNewBeneficiary()
        {
            var beneficiaryRequest = new BeneficiaryRequestModel { Nickname = "Nick1", UserId = 1 };
            var user = new User { Id = 1, Beneficiaries = new List<Beneficiary>() };

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(beneficiaryRequest.UserId))
                         .ReturnsAsync(user);
            _mockBeneficiaryRepo.Setup(repo => repo.AddBeneficiaryAsync(It.IsAny<Beneficiary>()))
                                .Returns(Task.CompletedTask);

            var result = await _beneficiaryService.CreateBeneficiaryAsync(beneficiaryRequest);

            Assert.NotNull(result);
            Assert.Equal(beneficiaryRequest.Nickname, result.Nickname);
            Assert.Equal(beneficiaryRequest.UserId, result.UserId);
        }
    }
}
