using CeitineCheeseAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CeitineCheeseAPI.Controllers
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
