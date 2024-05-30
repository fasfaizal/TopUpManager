using TopUpManager.Common.Entity;
using TopUpManager.Common.Models.Request;

namespace TopUpManager.Common.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(UserRequestModel user);
    }
}
