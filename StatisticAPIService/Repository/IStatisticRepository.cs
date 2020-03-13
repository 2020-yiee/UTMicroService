using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StatisticAPIService.Repository
{
    public interface IStatisticRepository
    {
        object getMergeData(int webID, int eventType, string trackingUrl);
        void updateStatisticData();
        object updateStatisticData2();
    }
}
