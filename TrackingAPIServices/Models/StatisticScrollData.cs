using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingAPIServices.Models
{
    public class StatisticScrollData
    {
        public long documentHeight { get; set; }
        public List<double> scroll { get; set; }
    }
}
