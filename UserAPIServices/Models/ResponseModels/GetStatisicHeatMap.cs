using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPIServices.Models.ResponseModels
{
    public class GetStatisicHeatMap
    {
        public string click { get; set; }
        public string hover { get; set; }
        public string imageUrl { get; set; }
    }
}
