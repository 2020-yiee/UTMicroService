namespace HeatMapAPIServices.Repository
{
    public class StatisticData
    {
        public double x { get; set; }
        public double y { get; set; }

        public StatisticData(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}