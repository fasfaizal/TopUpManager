using Moq;
using TopUpManager.Common.Entity;
using TopUpManager.Common.Interfaces.DataAccess;
using TopUpManager.Services.Services;

namespace TopUpManager.Services.Tests.Services
{
    public class BeneficiaryServiceTests
    {
        private readonly Mock<IBeneficiaryRepo> _mockBeneficiaryRepo;
        private readonly BeneficiaryService _beneficiaryService;

        public BeneficiaryServiceTests()
        {
            _mockBeneficiaryRepo = new Mock<IBeneficiaryRepo>();
            _beneficiaryService = new BeneficiaryService(_mockBeneficiaryRepo.Object);
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
    }
}
