using System;
using System.Collections.Generic;

namespace AuthServer.EFModels
{
    public partial class TrackedData
    {
        public int TrackedDataId { get; set; }
        public int TrackingId { get; set; }
        public string Data { get; set; }

        public virtual TrackingInfor Tracking { get; set; }
    }
}
