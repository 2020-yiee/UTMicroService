﻿using System;
using System.Collections.Generic;

namespace CustomersAPIServices.EFModels
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
        public string ScreenWidth { get; set; }
        public string ScreenHeight { get; set; }
    }
}
