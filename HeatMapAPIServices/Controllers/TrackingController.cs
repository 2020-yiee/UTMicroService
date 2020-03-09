using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeatMapAPIServices.EFModels;
using HeatMapAPIServices.Models;
using HeatMapAPIServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace HeatMapAPIServices.Controllers
{
    [Route("api/tracking")]
    [ApiController]
    [EnableCors]
    public class TrackingController : ControllerBase
    {
        private IHeatmapRepository iRepository;

        [HttpPost("data")]
        public IActionResult saveData([FromBody] SaveDataRequest data)
        {
            iRepository = new HeatmapRepositoryImpl();
            Boolean result = iRepository.createDataStore(data);
            return Ok(result);
        }

        [HttpGet("data")]
        public IActionResult getData(int trackingId)
        {
            iRepository = new HeatmapRepositoryImpl();
            return Ok(iRepository.getData(trackingId));
        }

        [HttpDelete("data")]
        [Authorize(Roles ="admin")]
        public IActionResult deleteData([FromBody]DeleteDataRequest request)
        {
            iRepository = new HeatmapRepositoryImpl();
            bool  result = iRepository.deleteData(request);
            if(result) return Ok();
            return NotFound();
        }

        [HttpPost("check")]
        public IActionResult checkTrackingType([FromBody] checkingRequest request)
        {
            iRepository = new HeatmapRepositoryImpl();
            TrackingInforResponse infor = iRepository.checkTrackingType(request);
            if (infor != null) return Ok(infor);
            return NotFound();
        }

        //==================================================================================

        [HttpPost("info")]
        [Authorize]
        public IActionResult createTrackingInfo([FromBody]CreateTrackingInforRequest request)
        {
            iRepository = new HeatmapRepositoryImpl();

            bool result = iRepository.createTrackingInfor(request);
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
            iRepository = new HeatmapRepositoryImpl();

            bool result = iRepository.updateTrackingInfor(request);
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
            iRepository = new HeatmapRepositoryImpl();

            bool result = iRepository.deleteTrackingInfor(trackingId);
            if (result)
            {
                return Ok();
            }
            else return BadRequest();
        }
    }
}