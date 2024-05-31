using Microsoft.AspNetCore.Mvc;
using TopUpManager.Common.Interfaces.Services;

namespace TopUpManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopUpTransactionsController : ControllerBase
    {
        private readonly ITopUpTransactionService _topUpTransactionService;

        public TopUpTransactionsController(ITopUpTransactionService topUpTransactionService)
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
