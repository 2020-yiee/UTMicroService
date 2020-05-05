using System;
using System.Collections.Generic;

namespace TrackingAPIServices.EFModels
{
    public partial class TrackedHeatmapData
    {
        public int TrackedHeatmapDataId { get; set; }
        public string TrackingUrl { get; set; }
        public int WebId { get; set; }
        public string Data { get; set; }
        public int EventType { get; set; }
        public long CreatedAt { get; set; }
        public string SessionId { get; set; }
        public long? ScreenHeight { get; set; }
        public long? ScreenWidth { get; set; }

        public virtual Website Web { get; set; }
        public virtual StatisticHeatmap StatisticHeatmap { get; set; }
    }
}
