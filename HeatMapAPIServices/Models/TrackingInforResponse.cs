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
        public string trackingType { get; set; }
        public bool isRemoved { get; set; }

        public TrackingInforResponse(int trackingId, int webId, string trackingUrl, string trackingType, bool isRemoved)
        {
            this.trackingId = trackingId;
            this.webId = webId;
            this.trackingUrl = trackingUrl;
            this.trackingType = trackingType;
            this.isRemoved = isRemoved;
        }
    }
}
