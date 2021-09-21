using CheeseAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CheeseAPI.Controllers
{
    [ApiController]
    [Route("api/stats")]
    public class StatsController : Controller
    {
        public class StatsScheme
        {
            public int CheesePagesCount { get; set; }
            public DateTime LastUpdate { get; set; }
        }

        [HttpGet]
        public ActionResult<StatsScheme> Get()
        {
            return new StatsScheme()
            {
                CheesePagesCount = CheeseService.GetPagesCount(),
                LastUpdate = CheeseService.LastCheesePoolUpdate
            };
        }
    }
}
