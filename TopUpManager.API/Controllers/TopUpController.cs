using Microsoft.AspNetCore.Mvc;
using TopUpManager.Common.Exceptions;
using TopUpManager.Common.Interfaces.Services;
using TopUpManager.Common.Models.Request;

namespace TopUpManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopUpController : ControllerBase
    {
        private readonly ITopUpService _topUpTransactionService;
        private readonly ILogger _logger;

        public TopUpController(ITopUpService topUpTransactionService, ILogger<TopUpController> logger)
        {
            _topUpTransactionService = topUpTransactionService;
            _logger = logger;
        }

        /// <summary>
        /// Handles HTTP GET requests to retrieve a list of options for top-up transactions.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that contains an HTTP 200 OK response with a list of options.
        /// </returns>
        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var options = _topUpTransactionService.GetOptions();
            return Ok(options);
        }


        /// <summary>
        /// Handles HTTP POST requests to initiate a top-up transaction.
        /// </summary>
        /// <param name="topUpRequest">Object containing information about the top-up transaction.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the top-up transaction initiation.
        /// </returns>
        /// <remarks>
        /// If the top-up is successful, it returns an HTTP 201 Created response.
        /// If an API exception occurs during the top-up process, it logs the exception and returns an appropriate HTTP status code along with the exception message.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Post(TopUpRequestModel topUpRequest)
        {
            try
            {
                await _topUpTransactionService.TopUpAsync(topUpRequest);
                return Created();
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }
    }
}
