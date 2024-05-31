using System.ComponentModel.DataAnnotations;

namespace TopUpManager.Common.Models.Request
{
    public class BeneficiaryRequestModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(20)]
        public string Nickname { get; set; }
    }
}
