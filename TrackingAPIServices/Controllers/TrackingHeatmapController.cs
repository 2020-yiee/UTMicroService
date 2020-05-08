using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeatMapAPIServices.Models;
using HeatMapAPIServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TrackingAPIServices.EFModels;
using TrackingAPIServices.Models;

namespace HeatMapAPIServices.Controllers
{
    [ApiController]
    [EnableCors]
    public class TrackingHeatmapController : ControllerBase
    {
        private ITrackingHeatmapRepository iRepository;

        public TrackingHeatmapController(ITrackingHeatmapRepository iRepository)
        {
            this.iRepository = iRepository;
        }

        protected int GetUserId()
        {
            try
            {
                return int.Parse(this.User.Claims.First(i => i.Type == "UserId").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //==============================tracked heatmap data=====================================================
        [HttpPost("api/tracked-data")]
        public IActionResult createTrackedHeatmapData([FromBody] SaveDataRequest data)
        {
            Boolean result = iRepository.createTrackedHeatmapData(data);
                if (result) return Ok();
                return BadRequest();   
        }
        //=================================statistic heatmap=====================================================
        [Authorize]
        [HttpGet("api/user/statistic/{webID}/{trackingInfoID}")]
        public IActionResult getStatisticHeatmap([FromRoute] int webID, [FromRoute] int trackingInfoID, int from, int to, int device)
        {
            return iRepository.getStatisticHeatMap(webID, trackingInfoID, from, to, device, GetUserId());
        }

        //=================================tracking heatmap info=================================================


        [HttpGet("api/tracking-info")]
        [Authorize]
        public IActionResult getTrackingHeatmapInfo(int webID)
        {
            return iRepository.getTrackingHeatmapInfo(webID, GetUserId());
        }

        [HttpGet("api/tracking-info/all-version")]
        [Authorize]
        public IActionResult getAllVersionTrackingHeatmapInfo(int trackingHeatmapInfoID)
        {
            return iRepository.getAllVersionTrackingHeatmapInfo(trackingHeatmapInfoID, GetUserId());
        }

        [HttpPost("api/tracking-info")]
        [Authorize]
        public IActionResult createTrackingInfo([FromBody]CreateTrackingHeatmapInforRequest request)
        {

            return iRepository.createHeatmapTrackingInfo(request, GetUserId());

        }

        [HttpPost("api/tracking-info/version")]
        [Authorize]
        public IActionResult createVersionTrackingInfo([FromBody]CreateVersionTrackingHeatmapInforRequest request)
        {

            return iRepository.createVersionHeatmapTrackingInfo(request, GetUserId());

        }

        [HttpPut("api/tracking-info")]
        [Authorize]
        public IActionResult updateTrackingInfo([FromBody]UpdateTrackingHeatmapInforRequest request)
        {

            var result = iRepository.updateTrackingHeatmapInfo(request, GetUserId());
            if (result != null)
            {
                return Ok(result);
            }
            else return BadRequest();
        }

        [HttpDelete("api/tracking-info")]
        [Authorize]
        public IActionResult deleteTrackingInfo(int trackingHeatmapInfoID, bool isVersion)
        {
            bool result = iRepository.deleteTrackingHeatmapInfo(trackingHeatmapInfoID, GetUserId(), isVersion);
            if (result)
            {
                return Ok();
            }
            else return BadRequest();
        }
    }
}