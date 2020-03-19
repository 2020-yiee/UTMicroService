namespace HeatMapAPIServices.Controllers
{
    public class udpateTrackingStepInfoRequest
    {
        public int webID { get; set; }
        public int trackingFunnelInfoID { get; set; }
        public object steps { get; set; }
    }
}