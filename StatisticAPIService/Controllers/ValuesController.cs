using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StatisticAPIService.Repository;

namespace StatisticAPIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IStatisticRepository repository;

        public ValuesController(IStatisticRepository repository)
        {
            this.repository = repository;
        }


        [HttpGet]
        public ActionResult Get(int webID, int eventType,string trackingUrl)
        {
            return Ok(repository.getMergeData(webID, eventType,trackingUrl));
        }
        [HttpGet("2")]
        public ActionResult Get2()
        {
            return Ok(repository.updateStatisticData2());
        }
    }
}
