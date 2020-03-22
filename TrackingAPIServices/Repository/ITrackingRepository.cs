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
        IActionResult getCheckingHeatmapInfo(int websiteId, int userId);
        IActionResult createHeatmapTrackingInfor(CreateTrackingHeatmapInforRequest request, int userId);
        TrackingHeatmapInfo updateTrackingHeatmapInfor(UpdateTrackingHeatmapInforRequest request, int userId);
        bool deleteTrackingFunnelInfo(int trackingId, int userId);
        IEnumerable<TrackingFunnelInfo> getCheckingFunnelInfo(int webID, int v);
        IActionResult createFunnelTrackingInfo(CreateTrackingFunnelInforRequest request, int v);
        bool createTrackedFunnelData(SaveFunnelDataRequest data);
        List<TrackedFunnelData> getTrackedFunnelData(int webID);
        List<StatisticHeatmap> getstatisticHeatmapData(int trackingHeatmapInfoID);
        List<StatisticFunnel> getstatisticFunnelData(int trackingFunnelInfoID);
        object udpateFunnelTrackingInfo(udpateTrackingStepInfoRequest request, int v);
        object getStatisticHeatMap(int webID, int trackingInfoID, int from, int to, int device, int v);
        Object getStatisticFunnel(int webID, int trackingFunnelInfoID, int from, int to, int userId);
        IActionResult udpateNameFunnelTrackingInfo(udpateTrackingNameInfoRequest request, int v);
    }
}
