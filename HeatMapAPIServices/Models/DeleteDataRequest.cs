using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class DeleteDataRequest
    {
        public int webId { get; set; }
        public string type { get; set; }
    }
}
