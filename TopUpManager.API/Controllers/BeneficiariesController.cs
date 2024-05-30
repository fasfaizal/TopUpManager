using Microsoft.AspNetCore.Mvc;
using TopUpManager.Common.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TopUpManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiariesController : ControllerBase
    {
        private readonly IBeneficiaryService _beneficiaryService;
        public BeneficiariesController(IBeneficiaryService beneficiaryService)
        {
            _beneficiaryService = beneficiaryService;
        }


        /// <summary>
        /// Retrieves a list of beneficiaries for the specified user.
        /// </summary>
        /// <param name="userId">
        /// The ID of the user for whom to retrieve beneficiaries.
        /// </param>
        /// <returns>
        /// List of beneficiaries for the specified user.
        /// </returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var beneficiaries = await _beneficiaryService.GetBeneficiariesAsync(userId);
            return Ok(beneficiaries);
        }
    }
}
