using StatisticAPIService.Models;
using System.Collections.Generic;

namespace HeatMapAPIServices.Controllers
{
    public class CreateTrackingFunnelInforRequest
    {
        public int webID { get; set; }
        public string name { get; set; }
        public List<Step> steps { get; set; }
        
    }
}