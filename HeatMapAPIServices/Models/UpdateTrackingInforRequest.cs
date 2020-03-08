using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class UpdateTrackingInforRequest
    {
        public int trackingId { get; set; }
        public int webId { get; set; }
        public string trackingUrl { get; set; }
        public string trackingType { get; set; }
    }
}
