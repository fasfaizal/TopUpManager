using Microsoft.AspNetCore.Mvc;

namespace FinanceService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        [HttpGet("balance")]
        public IActionResult GetBalance()
        {
            // Implement get balance functionality
            // In real the return value could be an object containing other fields as well
            return Ok(100);
        }

        [HttpPost("debit")]
        public IActionResult Debit()
        {
            // Implement debit functionality
            return Ok();
        }

        [HttpPost("credit")]
        public IActionResult Credit()
        {
            // Implement credit functionality
            return Ok();
        }
    }
}
