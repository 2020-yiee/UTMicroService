using System;
using System.Collections.Generic;

namespace HeatMapAPIServices.EFModels
{
    public partial class TrackingInfor
    {
        public TrackingInfor()
        {
            TrackedData = new HashSet<TrackedData>();
        }

        public int TrackingId { get; set; }
        public int WebId { get; set; }
        public string TrackingUrl { get; set; }
        public string TrackingType { get; set; }
        public bool IsRemoved { get; set; }

        public virtual Website Web { get; set; }
        public virtual ICollection<TrackedData> TrackedData { get; set; }
    }
}
