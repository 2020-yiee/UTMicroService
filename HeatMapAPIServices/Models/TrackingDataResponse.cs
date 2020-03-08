using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Models
{
    public class TrackingDataResponse
    {
        public int tracked_data_id { get; set; }
        public int tracking_id { get; set; }
        public string data { get; set; }

        public TrackingDataResponse(int tracked_data_id, int tracking_id, string data)
        {
            this.tracked_data_id = tracked_data_id;
            this.tracking_id = tracking_id;
            this.data = data;
        }
    }
}
