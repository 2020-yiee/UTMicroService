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
        private ITrackingRepository iRepository;

        public TrackingController(ITrackingRepository iRepository)
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

        [HttpPost("api/tracked-data")]
        public IActionResult createTrackedHeatmapData([FromBody] SaveDataRequest data)
        {
            Boolean result = iRepository.createTrackedHeatmapData(data);
            if (result) return Ok();
            return BadRequest();
        }


        //=================================statistic heatmap=====================================================


        [HttpGet("api/statistic-heatmap-data")]
        public IActionResult getStatisticHeatmapData(int trackingHeatmapInfoID)
        {
            List<StatisticHeatmap> result = iRepository.getstatisticHeatmapData(trackingHeatmapInfoID);
            if (result == null || result.Count == 0) return BadRequest();
            return Ok(result);
        }

        //=================================tracking heatmap info=================================================


        [HttpGet("api/tracking-info")]
        [Authorize]
        public IActionResult getTrackingHeatmapInfo(int webID)
        {
            List<TrackingHeatmapInfo> infor = iRepository.getCheckingHeatmapInfo(webID, GetUserId()).ToList();
            if (infor != null && infor.Count > 0) return Ok(infor);
            return BadRequest();
        }

        [HttpPost("api/tracking-info")]
        [Authorize]
        public IActionResult createTrackingInfo([FromBody]CreateTrackingHeatmapInforRequest request)
        {

            var result = iRepository.createHeatmapTrackingInfor(request, GetUserId());
            if (result != null)
            {
                return Ok(result);
            }
            else return BadRequest();
        }

        [HttpPut("api/tracking-info")]
        [Authorize]
        public IActionResult updateTrackingInfo([FromBody]UpdateTrackingHeatmapInforRequest request)
        {

            var result = iRepository.updateTrackingHeatmapInfor(request, GetUserId());
            if (result != null)
            {
                return Ok(result);
            }
            else return BadRequest();
        }

        [HttpDelete("api/tracking-info")]
        [Authorize]
        public IActionResult deleteTrackingInfo(int trackingHeatmapInfoID)
        {

            bool result = iRepository.deleteTrackingFunnelInfo(trackingHeatmapInfoID, GetUserId());
            if (result)
            {
                return Ok();
            }
            else return BadRequest();
        }

        //=============================================tracking funnel info=========================================

        [HttpGet("api/funnel-info")]
        [Authorize]
        public IActionResult getTrackingFunnelInfo(int webID)
        {
            List<TrackingFunnelInfo> infor = iRepository.getCheckingFunnelInfo(webID, GetUserId()).ToList();
            if (infor != null && infor.Count > 0) return Ok(infor);
            return BadRequest();
        }

        [HttpPost("api/funnel-info")]
        [Authorize]
        public IActionResult createTrackingFunnelInfo([FromBody]CreateTrackingFunnelInforRequest request)
        {

            var result = iRepository.createFunnelTrackingInfo(request, GetUserId());
            if (result != null)
            {
                return Ok(result);
            }
            else return BadRequest();
        }

        [HttpPut("api/funnel-info")]
        [Authorize]
        public IActionResult updateNameStepsTrackingFunnelInfo([FromBody] udpateTrackingInfoRequest request)
        {
            var result = iRepository.udpateFunnelTrackingInfo(request, GetUserId());
            if (result != null)
            {
                return Ok(result);
            }
            else return BadRequest();
        }

        
        [HttpDelete("api/funnel-info")]
        [Authorize]
        public IActionResult deleteTrackingFunnelInfo(int trackingFunnelInfoID)
        {
            
            bool result = iRepository.deleteTrackingFunnelInfo(trackingFunnelInfoID, GetUserId());
            if (result)
            {
                return Ok();
            }
            else return BadRequest();
        }

        //========================tracked funnel data=========================================
        [HttpPost("api/tracked-funnel-data")]
        public IActionResult createTrackedFunnelData([FromBody] SaveFunnelDataRequest data)
        {
            Boolean result = iRepository.createTrackedFunnelData(data);
            if (result) return Ok();
            return BadRequest();
        }

        

        //=================================statistic funnel=====================================================
        [HttpGet("api/statistic-funnel-data")]
        public IActionResult getStatisticFunnelData(int trackingFunnelInfoID)
        {
            List<StatisticFunnel> result = (List<StatisticFunnel>)iRepository.getstatisticFunnelData(trackingFunnelInfoID);
            if (result == null || result.Count == 0) return BadRequest();
            return Ok(result);
        }
    }
}