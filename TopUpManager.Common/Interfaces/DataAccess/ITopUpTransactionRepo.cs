using TopUpManager.Common.Entity;

namespace TopUpManager.Common.Interfaces.DataAccess
{
    public interface ITopUpTransactionRepo
    {
        Task AddTransactionAsync(TopUpTransaction topUpTransaction);
    }
}
