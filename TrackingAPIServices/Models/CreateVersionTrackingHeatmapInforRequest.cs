﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingAPIServices.Models
{
    public class CreateVersionTrackingHeatmapInforRequest
    {
        public int TrackingHeatmapInfoId { get; set; }
        public string captureUrl { get; set; }
    }
}
