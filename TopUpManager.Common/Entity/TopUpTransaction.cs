using System.ComponentModel.DataAnnotations.Schema;

namespace TopUpManager.Common.Entity
{
    public class TopUpTransaction
    {
        public int Id { get; set; }
        public int BeneficiaryId { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Beneficiary Beneficiary { get; set; }
    }
}
