using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class UpdateTrackingHeatmapInforRequest
    {
        public int trackingHeatmapInfoID { get; set; }
        public int webID { get; set; }
        public string trackingUrl { get; set; }
    }
}
