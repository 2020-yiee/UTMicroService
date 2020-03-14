using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticService.Models
{
    class TempData
    {
        public string selector { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int offsetX { get; set; }
        public int offsetY { get; set; }

        public TempData(string selector, int width, int height, int offsetX, int offsetY)
        {
            this.selector = selector;
            this.width = width;
            this.height = height;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }
    }
}
