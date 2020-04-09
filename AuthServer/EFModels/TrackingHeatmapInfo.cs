using System;
using System.Collections.Generic;

namespace AuthServer.EFModels
{
    public partial class TrackingHeatmapInfo
    {
        public int TrackingHeatmapInfoId { get; set; }
        public int WebId { get; set; }
        public string TrackingUrl { get; set; }
        public bool Removed { get; set; }
        public string Name { get; set; }
        public long CreatedAt { get; set; }
        public string LgImageUrl { get; set; }
        public string MdImageUrl { get; set; }
        public string SmImageUrl { get; set; }
        public string TypeUrl { get; set; }
        public int AuthorId { get; set; }
    }
}
