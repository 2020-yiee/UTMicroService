namespace HeatMapAPIServices.Controllers
{
    public class CreateTrackingFunnelInforRequest
    {
        public int webID { get; set; }
        public string name { get; set; }
        public object steps { get; set; }
        
    }
}