namespace TopUpManager.Common.Interfaces.Services
{
    public interface IExternalFinanceService
    {
        Task<decimal> GetBalance(int userId);
        Task Debit(int userId, decimal amount);
        Task Credit(int userId, decimal amount);
    }
}
