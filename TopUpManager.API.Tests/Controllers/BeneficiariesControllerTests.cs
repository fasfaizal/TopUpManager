using Microsoft.AspNetCore.Mvc;
using Moq;
using TopUpManager.API.Controllers;
using TopUpManager.Common.Entity;
using TopUpManager.Common.Interfaces.Services;

namespace TopUpManager.API.Tests.Controllers
{
    public class BeneficiariesControllerTests
    {
        private readonly Mock<IBeneficiaryService> _mockBeneficiaryService;
        private readonly BeneficiariesController _beneficiariesController;

        public BeneficiariesControllerTests()
        {
            _mockBeneficiaryService = new Mock<IBeneficiaryService>();
            _beneficiariesController = new BeneficiariesController(_mockBeneficiaryService.Object);
        }

        [Fact]
        public async Task Get_ValidUserId_ReturnsOkResultWithBeneficiaries()
        {
            int userId = 1;
            var expectedBeneficiaries = new List<Beneficiary>
            {
                new Beneficiary { Id = 1, Nickname = "Beneficiary 1", UserId = userId },
                new Beneficiary { Id = 2, Nickname = "Beneficiary 2" ,UserId = userId },
            };
            _mockBeneficiaryService.Setup(service => service.GetBeneficiariesAsync(userId))
                                   .ReturnsAsync(expectedBeneficiaries);

            var result = await _beneficiariesController.Get(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBeneficiaries = Assert.IsAssignableFrom<List<Beneficiary>>(okResult.Value);
            Assert.Equal(expectedBeneficiaries, returnedBeneficiaries);
        }
    }
}
