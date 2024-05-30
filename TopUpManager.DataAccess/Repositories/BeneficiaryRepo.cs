using Microsoft.EntityFrameworkCore;
using TopUpManager.Common.Entity;
using TopUpManager.Common.Interfaces.DataAccess;
using TopUpManager.DataAccess.DBContext;

namespace TopUpManager.DataAccess.Repositories
{
    public class BeneficiaryRepo : IBeneficiaryRepo
    {
        private readonly TopUpManagerDbContext _dbContext;

        public BeneficiaryRepo(TopUpManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves a list of beneficiaries for the specified user asynchronously.
        /// </summary>
        /// <param name="userId">
        /// The ID of the user for whom to retrieve beneficiaries.
        /// </param>
        /// <returns>
        /// List of beneficiaries for a user.
        /// </returns>
        public async Task<List<Beneficiary>> GetBeneficiariesAsync(int userId)
        {
            return await _dbContext.Beneficiaries.Where(beneficiary => beneficiary.UserId == userId).ToListAsync();
        }
    }
}
