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

        /// <summary>
        /// Adds a new user to the database asynchronously.
        /// </summary>
        /// <param name="user">
        /// The user object to add to the database.
        /// </param>
        public async Task AddUserAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
