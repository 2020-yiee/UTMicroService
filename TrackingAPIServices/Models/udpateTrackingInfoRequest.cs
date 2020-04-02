using StatisticAPIService.Models;
using System.Collections.Generic;

namespace HeatMapAPIServices.Controllers
{
    public class udpateTrackingStepInfoRequest
    {
        public int trackingFunnelInfoID { get; set; }
        public string newName { get; set; }
        public List<Step> steps { get; set; }
    }
}