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


        [HttpGet("api/tracked-funnel-data")]
        public IActionResult get(int ID)
        {
            using (var context = new DBUTContext())
            {
                return Ok(context.TrackedFunnelData.FirstOrDefault(s => s.TrackedFunnelDataId == ID));
            }
        }



        //=================================statistic heatmap=====================================================


        [HttpGet("api/statistic-heatmap-data")]
        public IActionResult getStatisticHeatmapData(int trackedHeatmapInfoID)
        {
            List<StatisticHeatmap> result = iRepository.getstatisticHeatmapData(trackedHeatmapInfoID);
            if (result == null || result.Count == 0) return BadRequest();
            return Ok(result);
        }

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

        //=============================================tracking funnel info=========================================

        [HttpGet("api/funnel-info")]
        [Authorize]
        public IActionResult getTrackingFunnelInfo(int webID)
        {
            List<TrackingFunnelInfoResponse> infor = iRepository.getTrackingFunnelInfo(webID, GetUserId()).ToList();
            if (infor != null) return Ok(infor);
            return BadRequest();
        }

        [HttpPost("api/funnel-info")]
        [Authorize]
        public IActionResult createTrackingFunnelInfo([FromBody]CreateTrackingFunnelInforRequest request)
        {

            return iRepository.createFunnelTrackingInfo(request, GetUserId());

        }

        [HttpPut("api/funnel-info")]
        [Authorize]
        public IActionResult updateNameStepsTrackingFunnelInfo([FromBody] udpateTrackingStepInfoRequest request)
        {
            return iRepository.updateFunnelTrackingInfo(request, GetUserId());
        }

        //[HttpPut("api/funnel-info")]
        //[Authorize]
        //public IActionResult updateNamerackingFunnelInfo([FromBody] udpateTrackingNameInfoRequest request)
        //{
        //    return iRepository.udpateNameFunnelTrackingInfo(request, GetUserId());

        //}


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
        public IActionResult getStatisticFunnelData(int trackedFunnelInfoID)
        {
            List<StatisticFunnel> result = (List<StatisticFunnel>)iRepository.getstatisticFunnelData(trackedFunnelInfoID);
            if (result == null || result.Count == 0) return BadRequest();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("api/funnel/statistic/{webID}/{trackingInfoID}")]
        public IActionResult getStatisticFunnel([FromRoute] int webID, [FromRoute] int trackingInfoID, long from, long to)
        {
            return iRepository.getStatisticFunnel(webID, trackingInfoID, from, to, GetUserId());
        }
    }
}