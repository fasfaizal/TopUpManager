using TopUpManager.Common.Entity;
using TopUpManager.Common.Interfaces.DataAccess;
using TopUpManager.Common.Interfaces.Services;

namespace TopUpManager.Services.Services
{
    public class BeneficiaryService : IBeneficiaryService
    {
        private readonly IBeneficiaryRepo _beneficiaryRepo;

        public BeneficiaryService(IBeneficiaryRepo beneficiaryRepo)
        {
            _beneficiaryRepo = beneficiaryRepo;
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
            return await _beneficiaryRepo.GetBeneficiariesAsync(userId);
        }
    }
}
