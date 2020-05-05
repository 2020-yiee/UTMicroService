using System;
using System.Collections.Generic;

namespace AuthServer.EFModels
{
    public partial class StatisticFunnel
    {
        public int TrackedFunnelDataId { get; set; }
        public string StatisticData { get; set; }

        public virtual TrackedFunnelData TrackedFunnelData { get; set; }
    }
}
