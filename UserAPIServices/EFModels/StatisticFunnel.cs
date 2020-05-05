using System;
using System.Collections.Generic;

namespace UserAPIServices.EFModels
{
    public partial class StatisticFunnel
    {
        public int TrackedFunnelDataId { get; set; }
        public string StatisticData { get; set; }

        public virtual TrackedFunnelData TrackedFunnelData { get; set; }
    }
}
