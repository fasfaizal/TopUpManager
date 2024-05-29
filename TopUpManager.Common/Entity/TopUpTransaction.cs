namespace TopUpManager.Common.Entity
{
    public class TopUpTransaction
    {
        public int Id { get; set; }
        public int BeneficiaryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Beneficiary Beneficiary { get; set; }
    }
}
