using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class TrackingInforResponse
    {
        public int tracking_id { get; set; }
        public int web_id { get; set; }
        public string tracking_url { get; set; }
        public string tracking_type { get; set; }
        public bool is_removed { get; set; }

        public TrackingInforResponse(int tracking_id, int web_id, string tracking_url, string tracking_type, bool is_removed)
        {
            this.tracking_id = tracking_id;
            this.web_id = web_id;
            this.tracking_url = tracking_url;
            this.tracking_type = tracking_type;
            this.is_removed = is_removed;
        }
    }
}
