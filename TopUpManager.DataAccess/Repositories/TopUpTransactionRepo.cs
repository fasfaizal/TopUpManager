using TopUpManager.Common.Entity;
using TopUpManager.Common.Interfaces.DataAccess;
using TopUpManager.DataAccess.DBContext;

namespace TopUpManager.DataAccess.Repositories
{
    public class TopUpTransactionRepo : ITopUpTransactionRepo
    {
        private readonly TopUpManagerDbContext _dbContext;

        public TopUpTransactionRepo(TopUpManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds a top-up transaction asynchronously to the database.
        /// </summary>
        /// <param name="topUpTransaction">The object representing the top-up transaction to be added.</param>
        public async Task AddTransactionAsync(TopUpTransaction topUpTransaction)
        {
            await _dbContext.TopUpTransactions.AddAsync(topUpTransaction);
            await _dbContext.SaveChangesAsync();
        }
    }
}
