using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticService.Models
{
    class ScrollData
    {
        public double position { get; set; }
        public double duration { get; set; }

        public ScrollData(double position, double duration)
        {
            this.position = position;
            this.duration = duration;
        }
    }
}
