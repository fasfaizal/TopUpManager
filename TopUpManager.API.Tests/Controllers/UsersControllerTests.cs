using Microsoft.AspNetCore.Mvc;
using Moq;
using TopUpManager.API.Controllers;
using TopUpManager.Common.Entity;
using TopUpManager.Common.Interfaces.Services;
using TopUpManager.Common.Models.Request;

namespace TopUpManager.API.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UsersController _usersController;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _usersController = new UsersController(_mockUserService.Object);
        }

        [Fact]
        public async Task Post_WithValidUserRequestModel_ShouldReturnOkResult()
        {
            var userModel = new UserRequestModel { Name = "Test User", IsVerified = true };
            var user = new User { Id = 1, Name = "Test User", IsVerified = true };
            _mockUserService.Setup(service => service.CreateUserAsync(userModel)).ReturnsAsync(user);

            var result = await _usersController.Post(userModel);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(user.Id, returnedUser.Id);
            Assert.Equal(userModel.Name, returnedUser.Name);
            Assert.Equal(userModel.IsVerified, returnedUser.IsVerified);
        }

        [Fact]
        public async Task Post_WithInvalidModelState_ShouldReturnBadRequest()
        {
            var userModel = new UserRequestModel { Name = "", IsVerified = true };
            _usersController.ModelState.AddModelError("Name", "Name is required");

            var result = await _usersController.Post(userModel);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }
    }
}
