using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class SaveDataRequest
    {
        public string trackingUrl { get; set; }
        public int webId { get; set; }
        public string data { get; set; }
        public int type { get; set; }

    }
}
