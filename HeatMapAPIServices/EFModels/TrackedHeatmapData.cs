﻿using System;
using System.Collections.Generic;

namespace HeatMapAPIServices.EFModels
{
    public partial class TrackedHeatmapData
    {
        public int TrackedHeatmapDataId { get; set; }
        public string TrackingUrl { get; set; }
        public int WebId { get; set; }
        public string Data { get; set; }
        public int EventType { get; set; }
    }
}
