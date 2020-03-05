using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class CreateTrackingInforRequest
    {
        public int WebId { get; set; }
        public string TrackingUrl { get; set; }
        public string TrackingType { get; set; }
    }
}
