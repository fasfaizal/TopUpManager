using TopUpManager.Common.Entity;

namespace TopUpManager.Common.Interfaces.Services
{
    public interface IBeneficiaryService
    {
        Task<List<Beneficiary>> GetBeneficiariesAsync(int userId);
    }
}
