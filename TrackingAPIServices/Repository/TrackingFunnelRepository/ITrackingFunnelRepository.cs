using HeatMapAPIServices.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingAPIServices.Models;

namespace TrackingAPIServices.Repository.TrackingFunnelRepository
{
    public interface ITrackingFunnelRepository
    {
        IEnumerable<TrackingFunnelInfoResponse> getTrackingFunnelInfo(int webID, int v);
        IActionResult createFunnelTrackingInfo(CreateTrackingFunnelInforRequest request, int v);
        IActionResult updateFunnelTrackingInfo(udpateTrackingStepInfoRequest request, int v);
        bool deleteTrackingFunnelInfo(int trackingId, int userId);
        bool createTrackedFunnelData(SaveFunnelDataRequest data);
        IActionResult getStatisticFunnel(int webID, int trackingFunnelInfoID, long from, long to, int userId);

    }
}
