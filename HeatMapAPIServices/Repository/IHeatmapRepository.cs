using HeatMapAPIServices.EFModels;
using HeatMapAPIServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Repository
{
    interface IHeatmapRepository
    {
        Boolean createDataStore(SaveDataRequest data);
        IEnumerable<DataStore> getData(GetDataRequest request);
    }
}
