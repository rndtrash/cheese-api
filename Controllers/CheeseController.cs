using CheeseAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CheeseAPI.Controllers
{
    [ApiController]
    [Route("api/cheese")]
    public class CheeseController : ControllerBase
    {

        private readonly ILogger<CheeseController> _logger;

        public CheeseController(ILogger<CheeseController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Get(0);
        }

        [HttpGet("{page}")]
        public ActionResult Get(int page)
        {
            var cheese = CheeseService.Get(page);

            if (cheese == null)
                return NotFound();

            return Ok(cheese);
        }
    }
}
