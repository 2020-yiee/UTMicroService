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
        IEnumerable<TrackingDataResponse> getData(GetDataRequest request);
        Boolean deleteData(DeleteDataRequest request);
        TrackingInforResponse checkTrackingType(checkingRequest request);
        bool createTrackingInfor(CreateTrackingInforRequest request);
        bool updateTrackingInfor(UpdateTrackingInforRequest request);
        bool deleteTrackingInfor(int trackingId);
    }
}
