using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class GetDataRequest
    {
        public int webId { get; set; }
        public string type { get; set; }
    }
}
