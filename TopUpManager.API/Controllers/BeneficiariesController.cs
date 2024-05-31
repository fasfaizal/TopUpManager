using Microsoft.AspNetCore.Mvc;
using TopUpManager.Common.Exceptions;
using TopUpManager.Common.Interfaces.Services;
using TopUpManager.Common.Models.Request;

namespace TopUpManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiariesController : ControllerBase
    {
        private readonly IBeneficiaryService _beneficiaryService;
        private readonly ILogger _logger;

        public BeneficiariesController(IBeneficiaryService beneficiaryService, ILogger<BeneficiariesController> logger)
        {
            _beneficiaryService = beneficiaryService;
            _logger = logger;
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

        /// <summary>
        /// Handles HTTP POST requests to create a new beneficiary.
        /// </summary>
        /// <param name="beneficiaryRequest">The request model containing the details of the beneficiary to be created.</param>
        /// <returns>
        /// On success, returns an HTTP 200 OK response with the created beneficiary.
        /// On failure, logs the exception and returns the appropriate HTTP status code with the error message.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Post(BeneficiaryRequestModel beneficiaryRequest)
        {
            try
            {
                var beneficiary = await _beneficiaryService.CreateBeneficiaryAsync(beneficiaryRequest);
                return Ok(beneficiary);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }
    }
}
