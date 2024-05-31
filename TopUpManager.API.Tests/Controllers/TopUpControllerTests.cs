using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using TopUpManager.API.Controllers;
using TopUpManager.Common.Exceptions;
using TopUpManager.Common.Interfaces.Services;
using TopUpManager.Common.Models.Request;

namespace TopUpManager.API.Tests.Controllers
{
    public class TopUpControllerTests
    {
        private readonly TopUpController _controller;
        private readonly Mock<ITopUpService> _mockTopUpService;
        private readonly Mock<ILogger<TopUpController>> _mockLogger;

        public TopUpControllerTests()
        {
            _mockTopUpService = new Mock<ITopUpService>();
            _mockLogger = new Mock<ILogger<TopUpController>>();
            _controller = new TopUpController(_mockTopUpService.Object, _mockLogger.Object);
        }

        [Fact]
        public void GetOptions_ReturnsOkResult_WithListOfOptions()
        {
            var options = new List<decimal> { 1, 2 };
            _mockTopUpService.Setup(service => service.GetOptions()).Returns(options);

            var result = _controller.GetOptions();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(options, okResult.Value);
        }

        [Fact]
        public async Task Post_SuccessfulTopUp_ReturnsCreatedResult()
        {
            var topUpRequest = new TopUpRequestModel();
            _mockTopUpService.Setup(service => service.TopUpAsync(topUpRequest)).Returns(Task.CompletedTask);

            var result = await _controller.Post(topUpRequest);

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task Post_ApiException_ReturnsErrorResponse()
        {
            var topUpRequest = new TopUpRequestModel();
            var apiException = new ApiException(HttpStatusCode.BadRequest, "Error message");
            _mockTopUpService.Setup(service => service.TopUpAsync(topUpRequest)).ThrowsAsync(apiException);

            var result = await _controller.Post(topUpRequest);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)apiException.StatusCode, statusCodeResult.StatusCode);
            Assert.Equal(apiException.Message, statusCodeResult.Value);
        }
    }
}
