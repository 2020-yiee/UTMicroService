using System;
using System.Collections.Generic;

namespace UserAPIServices.EFModels
{
    public partial class StatisticHeatmap
    {
        public int TrackedHeatmapDataId { get; set; }
        public string StatisticData { get; set; }

        public virtual TrackedHeatmapData TrackedHeatmapData { get; set; }
    }
}
