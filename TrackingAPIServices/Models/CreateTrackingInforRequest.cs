using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class CreateTrackingHeatmapInforRequest
    {
        public int webID { get; set; }
        public string name { get; set; }
        public string trackingUrl { get; set; }
        public string typeUrl { get; set; }
    }
}
