using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class SaveDataRequest
    {
        public string trackingUrl { get; set; }
        public int webID { get; set; }
        public string data { get; set; }
        public int eventType { get; set; }

    }
}
