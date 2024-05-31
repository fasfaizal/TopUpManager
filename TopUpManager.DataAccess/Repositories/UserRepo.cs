using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Retrieves a user by their ID, including their beneficiaries.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>
        /// An object representing the user with the specified ID, including their beneficiaries, 
        /// or null if no user with the specified ID is found.
        /// </returns>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _dbContext.Users
                .Where(user => user.Id == userId)
                .Include(user => user.Beneficiaries)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a user with their transactions asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="startDate">The start date from which to include the user's transactions.</param>
        /// <returns>
        /// Returns a User object with transactions, or null if no user is found.
        /// </returns>
        public async Task<User?> GetUserWithTransactionsAsync(int id, DateTime startDate)
        {
            return await _dbContext.Users
                .Where(user => user.Id == id)
                .Include(user => user.Beneficiaries)
                .ThenInclude(beneficiary => beneficiary.TopUpTransactions.Where(o => o.Date >= startDate))
                .FirstOrDefaultAsync();
        }
    }
}
