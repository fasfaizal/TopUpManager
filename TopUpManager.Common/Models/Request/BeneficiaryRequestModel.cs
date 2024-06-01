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
        [Required]
        [RegularExpression(@"^\+?[0-9]{1,15}$", ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }
    }
}
