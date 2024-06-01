namespace TopUpManager.Common.Configs
{
    public class Configurations
    {
        public int MaxBeneficiaryCount { get; set; }
        public List<decimal> TopUpOptions { get; set; }
        public decimal MonthlyLimitForUser { get; set; }
        public decimal VerifiedUserMonthlyBeneficiaryLimit { get; set; }
        public decimal UnverifiedUserMonthlyBeneficiaryLimit { get; set; }
        public decimal TransactionCharge { get; set; }
        public string FinanceServiceBaseUrl { get; set; }
    }
}
