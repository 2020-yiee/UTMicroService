﻿using HeatMapAPIServices.Controllers;
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
    public interface ITrackingHeatmapRepository
    {
        Boolean createTrackedHeatmapData(SaveDataRequest data);
        IEnumerable<TrackedHeatmapData> getTrackedHeatmapData(string trackingUrl, int type);
        IActionResult getTrackingHeatmapInfo(int websiteId, int userId);
        IActionResult getAllVersionTrackingHeatmapInfo(int trackingUrl, int userId);
        IActionResult createHeatmapTrackingInfo(CreateTrackingHeatmapInforRequest request, int userId);
        IActionResult updateTrackingHeatmapInfo(UpdateTrackingHeatmapInforRequest request, int userId);
        bool deleteTrackingHeatmapInfo(int trackingHeatmapInfoId, int userId, bool isVersion);
        List<TrackedFunnelData> getTrackedFunnelData(int webID);
        List<StatisticHeatmap> getstatisticHeatmapData(int trackingHeatmapInfoID);
        List<StatisticFunnel> getstatisticFunnelData(int trackingFunnelInfoID);
        IActionResult getStatisticHeatMap(int webID, int trackingInfoID, int from, int to, int device, int v);
        IActionResult createVersionHeatmapTrackingInfo(CreateVersionTrackingHeatmapInforRequest request, int v);
    }
}
