using System;
using System.Collections.Generic;

namespace CustomersAPIServices.EFModels
{
    public partial class TrackedFunnelData
    {
        public int TrackedFunnelDataId { get; set; }
        public string SessionId { get; set; }
        public int WebId { get; set; }
        public string TrackedSteps { get; set; }
    }
}
