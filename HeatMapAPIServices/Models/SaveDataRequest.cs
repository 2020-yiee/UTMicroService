using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class SaveDataRequest
    {
        public int WebId { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
    }
}
