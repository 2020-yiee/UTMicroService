using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class SaveDataRequest
    {
        public int trackingId { get; set; }
        public string data { get; set; }
    }
}
