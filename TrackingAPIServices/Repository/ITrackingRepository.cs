using HeatMapAPIServices.Controllers;
using HeatMapAPIServices.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingAPIServices.EFModels;
using TrackingAPIServices.Models;

namespace HeatMapAPIServices.Repository
{
    public interface ITrackingRepository
    {
        Boolean createTrackedHeatmapData(SaveDataRequest data);
        IEnumerable<TrackedHeatmapData> getTrackedHeatmapData(string trackingUrl, int type);
        IActionResult getTrackingHeatmapInfo(int websiteId, int userId);
        IActionResult createHeatmapTrackingInfo(CreateTrackingHeatmapInforRequest request, int userId);
        IActionResult updateTrackingHeatmapInfo(UpdateTrackingHeatmapInforRequest request, int userId);
        bool deleteTrackingHeatmapInfo(int trackingHeatmapInfoId, int userId);
        bool deleteTrackingFunnelInfo(int trackingId, int userId);
        IEnumerable<TrackingFunnelInfoResponse> getTrackingFunnelInfo(int webID, int v);
        IActionResult createFunnelTrackingInfo(CreateTrackingFunnelInforRequest request, int v);
        bool createTrackedFunnelData(SaveFunnelDataRequest data);
        List<TrackedFunnelData> getTrackedFunnelData(int webID);
        List<StatisticHeatmap> getstatisticHeatmapData(int trackingHeatmapInfoID);
        List<StatisticFunnel> getstatisticFunnelData(int trackingFunnelInfoID);
        IActionResult updateFunnelTrackingInfo(udpateTrackingStepInfoRequest request, int v);
        IActionResult getStatisticHeatMap(int webID, int trackingInfoID, int from, int to, int device, int v);
        IActionResult getStatisticFunnel(int webID, int trackingFunnelInfoID, long from, long to, int userId);  
    }
}
