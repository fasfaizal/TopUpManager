using Moq;
using TopUpManager.Common.Entity;
using TopUpManager.Common.Interfaces.DataAccess;
using TopUpManager.Common.Models.Request;
using TopUpManager.Services.Services;

namespace TopUpManager.Services.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepo>();
            _userService = new UserService(_mockUserRepo.Object);
        }

        [Fact]
        public async Task CreateUserAsync_WithValidUserRequestModel_ShouldReturnNewUser()
        {
            var userModel = new UserRequestModel { Name = "Test User", IsVerified = true };

            var result = await _userService.CreateUserAsync(userModel);

            Assert.NotNull(result);
            Assert.Equal(userModel.Name, result.Name);
            Assert.Equal(userModel.IsVerified, result.IsVerified);
            _mockUserRepo.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_WithNullUserRequestModel_ShouldThrowArgumentNullException()
        {
            // Arrange
            UserRequestModel userModel = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.CreateUserAsync(userModel));
        }
    }
}
