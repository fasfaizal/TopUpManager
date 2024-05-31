using TopUpManager.Common.Entity;
using TopUpManager.Common.Models.Request;

namespace TopUpManager.Common.Interfaces.Services
{
    public interface IBeneficiaryService
    {
        Task<List<Beneficiary>> GetBeneficiariesAsync(int userId);
        Task<Beneficiary> CreateBeneficiaryAsync(BeneficiaryRequestModel beneficiaryRequest);
    }
}
