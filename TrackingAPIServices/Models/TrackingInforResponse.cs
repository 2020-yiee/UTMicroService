using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class TrackingInforResponse
    {
        public int trackingId { get; set; }
        public int webId { get; set; }
        public string trackingUrl { get; set; }
        public bool removed { get; set; }

        public TrackingInforResponse(int trackingId, int webId, string trackingUrl, bool isRemoved)
        {
            this.trackingId = trackingId;
            this.webId = webId;
            this.trackingUrl = trackingUrl;
            this.removed = isRemoved;
        }
    }
}
