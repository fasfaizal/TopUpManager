using TopUpManager.Common.Models.Request;

namespace TopUpManager.Common.Interfaces.Services
{
    public interface ITopUpService
    {
        List<decimal> GetOptions();
        Task TopUpAsync(TopUpRequestModel topUpRequest);
    }
}
