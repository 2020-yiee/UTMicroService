using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingAPIServices.Models
{
    public class TrackingFunnelInfoResponse
    {
        public int trackingFunnelInfoId { get; set; }
        public int webId { get; set; }
        public string name { get; set; }
        public string steps { get; set; }
        public bool removed { get; set; }
        public long createdAt { get; set; }
        public int authorId { get; set; }
        public string authorName { get; set; }
        public double conversionRate { get; set; }

        public TrackingFunnelInfoResponse(int trackingFunnelInfoId, int webId, string name, string steps, bool removed, long createdAt, int authorId, string authorName, double conversionRate)
        {
            this.trackingFunnelInfoId = trackingFunnelInfoId;
            this.webId = webId;
            this.name = name;
            this.steps = steps;
            this.removed = removed;
            this.createdAt = createdAt;
            this.authorId = authorId;
            this.authorName = authorName;
            this.conversionRate = conversionRate;
        }
    }
}
