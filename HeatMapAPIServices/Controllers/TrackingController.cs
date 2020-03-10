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
    [ApiController]
    [EnableCors]
    public class TrackingController : ControllerBase
    {
        private IHeatmapRepository iRepository;

        [HttpPost("api/tracked-data")]
        public IActionResult createTrackedHeatmapData([FromBody] SaveDataRequest data)
        {
            iRepository = new HeatmapRepositoryImpl();
            Boolean result = iRepository.createTrackedHeatmapData(data);
            if (result) return Ok();
            return BadRequest();
        }

        [HttpGet("api/tracked-data")]
        public IActionResult getTrackedHeatmapData(string trackingUrl, int eventType)
        {
            iRepository = new HeatmapRepositoryImpl();
            List<TrackedHeatmapData> result =(List<TrackedHeatmapData>) iRepository.getTrackedHeatmapData(trackingUrl, eventType);
            if (result == null || result.Count == 0) return BadRequest();
            return Ok(result);
        }

        [HttpDelete("api/tracked-data")]
        public IActionResult deleteTrackedHeatmapData([FromBody]DeleteDataRequest request)
        {
            iRepository = new HeatmapRepositoryImpl();
            bool  result = iRepository.deleteTrackedHeatmapData(request);
            if(result) return Ok();
            return BadRequest();
        }



        //==================================================================================


        [HttpGet("api/tracking-info")]
        public IActionResult getChekingHeatmapInfo( int webID)
        {
            iRepository = new HeatmapRepositoryImpl();
            List<TrackingInforResponse> infor = iRepository.getCheckingHeatmapInfo( webID).ToList();
            if (infor != null && infor.Count >0) return Ok(infor);
            return BadRequest();
        }

        [HttpPost("api/tracking-info")]
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

        [HttpPut("api/tracking-info")]
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

        [HttpDelete("api/tracking-info")]
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