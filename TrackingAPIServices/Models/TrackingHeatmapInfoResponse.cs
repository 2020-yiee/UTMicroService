using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingAPIServices.Models
{
    public class TrackingHeatmapInfoResponse
    {
        public int trackingHeatmapInfoId { get; set; }
        public string trackingUrl { get; set; }
        public string name { get; set; }
        public long createdAt { get; set; }
        public String authorName { get; set; }
        public int visit { get; set; }
        public string version { get; set; }

        public TrackingHeatmapInfoResponse(int trackingHeatmapInfoId, string trackingUrl, string name, long createdAt, string authorName, int visit, string version)
        {
            this.trackingHeatmapInfoId = trackingHeatmapInfoId;
            this.trackingUrl = trackingUrl;
            this.name = name;
            this.createdAt = createdAt;
            this.authorName = authorName;
            this.visit = visit;
            this.version = version;
        }
    }
}
