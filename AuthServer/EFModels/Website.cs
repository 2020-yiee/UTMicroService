using System;
using System.Collections.Generic;

namespace AuthServer.EFModels
{
    public partial class Website
    {
        public Website()
        {
            TrackedFunnelData = new HashSet<TrackedFunnelData>();
            TrackedHeatmapData = new HashSet<TrackedHeatmapData>();
            TrackingFunnelInfo = new HashSet<TrackingFunnelInfo>();
            TrackingHeatmapInfo = new HashSet<TrackingHeatmapInfo>();
        }

        public int WebId { get; set; }
        public string DomainUrl { get; set; }
        public bool Removed { get; set; }
        public int OrganizationId { get; set; }
        public bool Verified { get; set; }
        public long CreatedAt { get; set; }
        public int AuthorId { get; set; }

        public virtual User Author { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<TrackedFunnelData> TrackedFunnelData { get; set; }
        public virtual ICollection<TrackedHeatmapData> TrackedHeatmapData { get; set; }
        public virtual ICollection<TrackingFunnelInfo> TrackingFunnelInfo { get; set; }
        public virtual ICollection<TrackingHeatmapInfo> TrackingHeatmapInfo { get; set; }
    }
}
