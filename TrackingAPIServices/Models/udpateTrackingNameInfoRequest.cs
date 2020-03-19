using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingAPIServices.Models
{
    public class udpateTrackingNameInfoRequest
    {
        public int trackingFunnelInfoID { get; set; }
        public string newName { get; set; }
    }
}
