using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticService.Models
{
    class ResponseData
    {
        public int trackedHeatmapDataID { get; set; }
        public double x { get; set; }
        public double y { get; set; }

        public ResponseData(int trackedHeatmapDataID, double x, double y)
        {
            this.trackedHeatmapDataID = trackedHeatmapDataID;
            this.x = x;
            this.y = y;
        }
    }
}
