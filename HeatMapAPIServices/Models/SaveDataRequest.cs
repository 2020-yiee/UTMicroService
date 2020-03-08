using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class SaveDataRequest
    {
        public int tracking_id { get; set; }
        public string data { get; set; }
    }
}
