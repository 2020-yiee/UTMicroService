using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeatMapAPIServices.EFModels;
using HeatMapAPIServices.Models;
using HeatMapAPIServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeatMapAPIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private IHeatmapRepository repository;
        [HttpPost("data")]
        public IActionResult saveData([FromBody] SaveDataRequest data)
        {
            repository = new HeatmapRepositoryImpl();
            Boolean result = repository.createDataStore(data);
            return Ok(result);
        }

        [HttpGet("data")]
        public IActionResult getData([FromBody]GetDataRequest request)
        {
            repository = new HeatmapRepositoryImpl();
            return Ok(repository.getData(request));
        }

        [HttpDelete("data")]
        [Authorize(Roles ="admin")]
        public IActionResult deleteData([FromBody]DeleteDataRequest request)
        {
            repository = new HeatmapRepositoryImpl();
            return Ok(repository.deleteData(request));
        }

        [HttpPost("check")]
        public IActionResult checkTrackingType([FromBody] checkingRequest request)
        {
            repository = new HeatmapRepositoryImpl();
            TrackingInfor infor = repository.checkTrackingType(request);
            if (infor != null) return Ok(infor);
            return NotFound();
        }

        //==================================================================================

        [HttpPost("info")]
        [Authorize]
        public IActionResult createTrackingInfo([FromBody]CreateTrackingInforRequest request)
        {
            repository = new HeatmapRepositoryImpl();

            bool result = repository.createTrackingInfor(request);
            if (result)
            {
                return Ok();
            }
            else return BadRequest();
        }

        [HttpPut("info")]
        [Authorize]
        public IActionResult updateTrackingInfo([FromBody]UpdateTrackingInforRequest request)
        {
            repository = new HeatmapRepositoryImpl();

            bool result = repository.updateTrackingInfor(request);
            if (result)
            {
                return Ok();
            }
            else return BadRequest();
        }

        [HttpDelete("info")]
        [Authorize]
        public IActionResult deleteTrackingInfo(int trackingId)
        {
            repository = new HeatmapRepositoryImpl();

            bool result = repository.deleteTrackingInfor(trackingId);
            if (result)
            {
                return Ok();
            }
            else return BadRequest();
        }
    }
}