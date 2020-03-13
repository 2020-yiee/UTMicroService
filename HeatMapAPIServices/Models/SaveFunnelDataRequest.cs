namespace HeatMapAPIServices.Controllers
{
    public class SaveFunnelDataRequest
    {
        public string sessionID { get; set; }
        public int webID { get; set; }
        public string trackedSteps { get; set; }
    }
}