using TopUpManager.Common.Entity;

namespace TopUpManager.Common.Interfaces.DataAccess
{
    public interface IBeneficiaryRepo
    {
        Task<List<Beneficiary>> GetBeneficiariesAsync(int userId);
        Task AddBeneficiaryAsync(Beneficiary beneficiary);
    }
}
