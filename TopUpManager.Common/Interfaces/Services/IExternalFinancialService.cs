namespace TopUpManager.Common.Interfaces.Services
{
    public interface IExternalFinancialService
    {
        decimal GetBalance(int userId);
        void Debit(int userId, decimal amount);
    }
}
