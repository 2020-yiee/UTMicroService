﻿using System;
using System.Collections.Generic;

namespace HeatMapAPIServices.EFModels
{
    public partial class TrackingHeatmapInfo
    {
        public int TrackingHeatmapInfoId { get; set; }
        public int WebId { get; set; }
        public string TrackingUrl { get; set; }
        public bool Removed { get; set; }
    }
}