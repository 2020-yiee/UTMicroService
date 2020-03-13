using System;
using System.Collections.Generic;

namespace StatisticAPIService.EFModels
{
    public partial class StatisticHeatmap
    {
        public int TrackingHeatmapInfoId { get; set; }
        public string StatisticData { get; set; }
        public int? EventType { get; set; }
    }
}
