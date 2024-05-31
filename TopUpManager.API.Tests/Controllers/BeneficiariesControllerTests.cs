using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using TopUpManager.API.Controllers;
using TopUpManager.Common.Entity;
using TopUpManager.Common.Exceptions;
using TopUpManager.Common.Interfaces.Services;
using TopUpManager.Common.Models.Request;

namespace TopUpManager.API.Tests.Controllers
{
    public class BeneficiariesControllerTests
    {
        private readonly Mock<IBeneficiaryService> _mockBeneficiaryService;
        private readonly BeneficiariesController _beneficiariesController;
        private readonly Mock<ILogger<BeneficiariesController>> _mockLogger;

        public BeneficiariesControllerTests()
        {
            _mockBeneficiaryService = new Mock<IBeneficiaryService>();
            _mockLogger = new Mock<ILogger<BeneficiariesController>>();
            _beneficiariesController = new BeneficiariesController(_mockBeneficiaryService.Object, _mockLogger.Object);
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

        [Fact]
        public async Task Post_ValidBeneficiaryRequest_ReturnsOkResult()
        {
            var beneficiaryRequest = new BeneficiaryRequestModel { Nickname = "Nickname", UserId = 1 };
            var createdBeneficiary = new Beneficiary { Id = 1, Nickname = "Nickname", UserId = 1 };
            _mockBeneficiaryService.Setup(service => service.CreateBeneficiaryAsync(beneficiaryRequest))
                                   .ReturnsAsync(createdBeneficiary);

            var result = await _beneficiariesController.Post(beneficiaryRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBeneficiary = Assert.IsAssignableFrom<Beneficiary>(okResult.Value);
            Assert.Equal(createdBeneficiary, returnedBeneficiary);
        }

        [Fact]
        public async Task Post_ServiceThrowsApiException_ReturnsCorrectStatusCode()
        {
            var beneficiaryRequest = new BeneficiaryRequestModel { Nickname = "Nick", UserId = 1 };
            var apiException = new ApiException(HttpStatusCode.BadRequest, "Error occurred");
            _mockBeneficiaryService.Setup(service => service.CreateBeneficiaryAsync(beneficiaryRequest))
                                   .ThrowsAsync(apiException);

            var result = await _beneficiariesController.Post(beneficiaryRequest);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)System.Net.HttpStatusCode.BadRequest, statusCodeResult.StatusCode);
            Assert.Equal(apiException.Message, statusCodeResult.Value);
        }
    }
}
