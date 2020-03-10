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
        public IActionResult createTrackedHeatmapData([FromBody] SaveDataRequest data)
        {
            iRepository = new HeatmapRepositoryImpl();
            Boolean result = iRepository.createTrackedHeatmapData(data);
            return Ok(result);
        }

        [HttpGet("data")]
        public IActionResult getTrackedHeatmapData(string trackingUrl, int type)
        {
            iRepository = new HeatmapRepositoryImpl();
            List<TrackedHeatmapData> result =(List<TrackedHeatmapData>) iRepository.getTrackedHeatmapData(trackingUrl, type);
            if (result == null || result.Count == 0) return NotFound();
            return Ok(result);
        }

        [HttpDelete("data")]
        public IActionResult deleteTrackedHeatmapData([FromBody]DeleteDataRequest request)
        {
            iRepository = new HeatmapRepositoryImpl();
            bool  result = iRepository.deleteTrackedHeatmapData(request);
            if(result) return Ok();
            return NotFound();
        }



        //==================================================================================


        [HttpGet("info")]
        public IActionResult getChekingHeatmapInfo( int websiteId)
        {
            iRepository = new HeatmapRepositoryImpl();
            List<TrackingInforResponse> infor = iRepository.getCheckingHeatmapInfo( websiteId).ToList();
            if (infor != null && infor.Count >0) return Ok(infor);
            return NotFound();
        }

        [HttpPost("info")]
        public IActionResult createTrackingInfo([FromBody]CreateTrackingInforRequest request)
        {
            iRepository = new HeatmapRepositoryImpl();

            var result = iRepository.createHeatmapTrackingInfor(request);
            if (result!=null)
            {
                return Ok(result);
            }
            else return BadRequest();
        }

        [HttpPut("info")]
        public IActionResult updateTrackingInfo([FromBody]UpdateTrackingHeatmapInforRequest request)
        {
            iRepository = new HeatmapRepositoryImpl();

            var result = iRepository.updateTrackingHeatmapInfor(request);
            if (result!=null)
            {
                return Ok(result);
            }
            else return BadRequest();
        }

        [HttpDelete("info")]
        public IActionResult deleteTrackingInfo(int trackingHeatmapInfoID)
        {
            iRepository = new HeatmapRepositoryImpl();

            bool result = iRepository.deleteTrackingHeatmapInfor(trackingHeatmapInfoID);
            if (result)
            {
                return Ok();
            }
            else return BadRequest();
        }
    }
}