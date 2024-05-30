using System.ComponentModel.DataAnnotations;

namespace TopUpManager.Common.Models.Request
{
    public class UserRequestModel
    {
        [Required]
        public string Name { get; set; }
        public bool IsVerified { get; set; }
    }
}
