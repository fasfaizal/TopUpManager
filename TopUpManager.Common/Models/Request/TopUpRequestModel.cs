namespace TopUpManager.Common.Models.Request
{
    public class TopUpRequestModel
    {
        public int UserId { get; set; }
        public int BeneficiaryId { get; set; }
        public decimal Amount { get; set; }
    }
}
