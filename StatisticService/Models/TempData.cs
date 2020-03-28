using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticService.Models
{
    class data
    {
        public string selector { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public double offsetX { get; set; }
        public double offsetY { get; set; }

        public data(string selector, int width, int height, double offsetX, double offsetY)
        {
            this.selector = selector;
            this.width = width;
            this.height = height;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }
    }
}
