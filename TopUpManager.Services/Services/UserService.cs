using TopUpManager.Common.Entity;
using TopUpManager.Common.Interfaces.DataAccess;
using TopUpManager.Common.Interfaces.Services;
using TopUpManager.Common.Models.Request;

namespace TopUpManager.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User> CreateUserAsync(UserRequestModel userModel)
        {
            if (userModel is null)
            {
                throw new ArgumentNullException(nameof(userModel));
            }

            var newUser = new User { Name = userModel.Name, IsVerified = userModel.IsVerified };
            await _userRepo.AddUserAsync(newUser);
            return newUser;
        }
    }
}
