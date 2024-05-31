using Microsoft.AspNetCore.Mvc;
using TopUpManager.Common.Interfaces.Services;

namespace TopUpManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopUpController : ControllerBase
    {
        private readonly ITopUpService _topUpTransactionService;

        public TopUpController(ITopUpService topUpTransactionService)
        {
            _topUpTransactionService = topUpTransactionService;
        }

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var options = _topUpTransactionService.GetOptions();
            return Ok(options);
        }
    }
}
