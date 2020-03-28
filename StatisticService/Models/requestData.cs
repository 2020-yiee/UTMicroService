using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticService.Models
{
    class RequestData
    {
        public int trackedHeatmapDataID { get; set; }
        public string selector { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public double offsetX { get; set; }
        public double offsetY { get; set; }

        public RequestData(int trackedHeatmapDataID, string selector, int width, int height, double offsetX, double offsetY)
        {
            this.trackedHeatmapDataID = trackedHeatmapDataID;
            this.selector = selector;
            this.width = width;
            this.height = height;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public string ToStrings()
        {
            return "trackedHeatmapDataID:" + this.trackedHeatmapDataID + "\n"
                + "selector:" + this.selector + "\n"
                + "width:" + this.width + "\n"
                + "height:" + this.height + "\n"
                + "offsetX:" + this.offsetX + "\n"
                + "offsetY:" + this.offsetY + "\n";
        }
        
    }
}
