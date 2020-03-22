using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingAPIServices.Models
{
    public class StatisticFunnelResponse
    {
        public string stepName { get; set; }
        public string url { get; set; }
        public string typeUrl { get; set; }
        public int sessions { get; set; }

        public StatisticFunnelResponse(string stepName, string url, string typeUrl, int sessions)
        {
            this.stepName = stepName;
            this.url = url;
            this.typeUrl = typeUrl;
            this.sessions = sessions;
        }
    }
}
