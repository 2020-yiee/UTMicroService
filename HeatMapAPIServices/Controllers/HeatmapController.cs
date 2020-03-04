using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeatMapAPIServices.EFModels;
using HeatMapAPIServices.Models;
using HeatMapAPIServices.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HeatMapAPIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeatmapController : ControllerBase
    {
        private IHeatmapRepository repository;
        [HttpPost("data")]
        public IActionResult saveData(SaveDataRequest data)
        {
            repository = new HeatmapRepositoryImpl();
            Boolean result = repository.createDataStore(data);
            return Ok(result);
        }

        [HttpGet("data")]
        public IActionResult getData(GetDataRequest request)
        {
            repository = new HeatmapRepositoryImpl();
            return Ok(repository.getData(request));
        }
    }
}