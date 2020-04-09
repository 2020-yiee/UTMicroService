using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPIServices.Models
{
    public class StatisticData
    {
        public double x { get; set; }
        public double y { get; set; }

        public StatisticData(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
