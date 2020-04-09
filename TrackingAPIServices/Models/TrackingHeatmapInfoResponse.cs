using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingAPIServices.Models
{
    public class TrackingHeatmapInfoResponse
    {
        public int trackingHeatmapInfoId { get; set; }
        public int webId { get; set; }
        public string trackingUrl { get; set; }
        public bool removed { get; set; }
        public string name { get; set; }
        public long createdAt { get; set; }
        public string lgImageUrl { get; set; }
        public string mdImageUrl { get; set; }
        public string smImageUrl { get; set; }
        public string typeUrl { get; set; }
        public int authorId { get; set; }
        public String authorName { get; set; }
        public int visit { get; set; }

        public TrackingHeatmapInfoResponse(int trackingHeatmapInfoId, int webId, string trackingUrl, bool removed, string name, long createdAt, string lgImageUrl, string mdImageUrl, string smImageUrl, string typeUrl, int authorId, string authorName, int visit)
        {
            this.trackingHeatmapInfoId = trackingHeatmapInfoId;
            this.webId = webId;
            this.trackingUrl = trackingUrl;
            this.removed = removed;
            this.name = name;
            this.createdAt = createdAt;
            this.lgImageUrl = lgImageUrl;
            this.mdImageUrl = mdImageUrl;
            this.smImageUrl = smImageUrl;
            this.typeUrl = typeUrl;
            this.authorId = authorId;
            this.authorName = authorName;
            this.visit = visit;
        }
    }
}
