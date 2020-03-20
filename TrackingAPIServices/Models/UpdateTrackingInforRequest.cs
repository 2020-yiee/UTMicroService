using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class UpdateTrackingHeatmapInforRequest
    {
        public int trackingHeatmapInfoID { get; set; }
        public string newName { get; set; }
    }
}
