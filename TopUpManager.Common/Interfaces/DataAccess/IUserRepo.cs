using TopUpManager.Common.Entity;

namespace TopUpManager.Common.Interfaces.DataAccess
{
    public interface IUserRepo
    {
        Task AddUserAsync(User user);
        Task<User> GetUserByIdAsync(int id);
    }
}
