using System;
using System.Collections.Generic;

namespace AuthServer.EFModels
{
    public partial class StatisticHeatmap
    {
        public int TrackedHeatmapDataId { get; set; }
        public string StatisticData { get; set; }
    }
}
