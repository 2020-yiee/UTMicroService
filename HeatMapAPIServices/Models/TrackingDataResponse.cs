using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class TrackingDataResponse
    {
        public int trackedDataId { get; set; }
        public int trackingId { get; set; }
        public string data { get; set; }

        public TrackingDataResponse(int tracked_data_id, int tracking_id, string data)
        {
            this.trackedDataId = tracked_data_id;
            this.trackingId = tracking_id;
            this.data = data;
        }
    }
}
