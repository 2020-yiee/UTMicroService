using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class CreateTrackingInforRequest
    {
        public int web_id { get; set; }
        public string tracking_url { get; set; }
        public string tracking_type { get; set; }
    }
}
