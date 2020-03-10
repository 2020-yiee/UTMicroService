using HeatMapAPIServices.EFModels;
using HeatMapAPIServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Repository
{
    public interface IHeatmapRepository
    {
        Boolean createTrackedHeatmapData(SaveDataRequest data);
        IEnumerable<TrackedHeatmapData> getTrackedHeatmapData(string trackingUrl, int type);
        Boolean deleteTrackedHeatmapData(DeleteDataRequest request);
        IEnumerable<TrackingInforResponse> getCheckingHeatmapInfo(int websiteId);
        TrackingHeatmapInfo createHeatmapTrackingInfor(CreateTrackingInforRequest request);
        TrackingHeatmapInfo updateTrackingHeatmapInfor(UpdateTrackingHeatmapInforRequest request);
        bool deleteTrackingHeatmapInfor(int trackingId);
    }
}
