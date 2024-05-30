using TopUpManager.Common.Entity;
using TopUpManager.Common.Interfaces.DataAccess;
using TopUpManager.DataAccess.DBContext;

namespace TopUpManager.DataAccess.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly TopUpManagerDbContext _dbContext;

        public UserRepo(TopUpManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddUserAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
