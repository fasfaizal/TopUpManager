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

        /// <summary>
        /// Creates a new user asynchronously based on the provided user request model.
        /// </summary>
        /// <param name="userModel">
        /// The user request model containing the details of the user to be created.
        /// </param>
        /// <returns>
        /// Newly created user.
        /// </returns>
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
