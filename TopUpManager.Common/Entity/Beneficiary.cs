using System.ComponentModel.DataAnnotations;

namespace TopUpManager.Common.Entity
{
    public class Beneficiary
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string Nickname { get; set; }
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        public int UserId { get; set; }
        public ICollection<TopUpTransaction> TopUpTransactions { get; set; }
    }
}
