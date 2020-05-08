using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeatMapAPIServices.Controllers;
using HeatMapAPIServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrackingAPIServices.Models;
using TrackingAPIServices.Repository.TrackingFunnelRepository;

namespace TrackingAPIServices.Controllers
{
    [ApiController]
    [EnableCors]
    public class TrackingFunnelController : ControllerBase
    {

        private ITrackingFunnelRepository iRepository;

        public TrackingFunnelController(ITrackingFunnelRepository iRepository)
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
        [Authorize]
        [HttpGet("api/funnel/statistic/{webID}/{trackingInfoID}")]
        public IActionResult getStatisticFunnel([FromRoute] int webID, [FromRoute] int trackingInfoID, long from, long to)
        {
            return iRepository.getStatisticFunnel(webID, trackingInfoID, from, to, GetUserId());
        }
    }
}