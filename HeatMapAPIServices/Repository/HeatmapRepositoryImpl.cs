using HeatMapAPIServices.EFModels;
using HeatMapAPIServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Repository
{
    public class HeatmapRepositoryImpl : IHeatmapRepository
    {
        private readonly DBUTContext context = new DBUTContext();

        public TrackingInforResponse checkTrackingType(checkingRequest request)
        {
            try
            {
                TrackingInfor trackingInfo = context.TrackingInfor
                    .Where(s => s.WebId == request.webId)
                    .Where(s => s.TrackingUrl == request.trackingUrl)
                    .Where(s => s.IsRemoved == false)
                    .FirstOrDefault();

                return trackingInfo != null ? new TrackingInforResponse(trackingInfo.TrackingId, trackingInfo.WebId, trackingInfo.TrackingUrl, trackingInfo.TrackingType, trackingInfo.IsRemoved) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: "+ex.Message);
                return null;
            }
        }

        public bool createDataStore(SaveDataRequest data)
        {
            try
            {
                TrackedData trackedData = new TrackedData();
                trackedData.TrackingId = data.trackingId;
                trackedData.Data = data.data;
                context.TrackedData.Add(trackedData);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool createTrackingInfor(CreateTrackingInforRequest request)
        {
            TrackingInfor info = new TrackingInfor();
            info.WebId = request.webId;
            info.TrackingUrl = request.trackingUrl;
            info.TrackingType = request.trackingType;
            info.IsRemoved = false;
            try
            {
                context.TrackingInfor.Add(info);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }

        public bool deleteData(DeleteDataRequest request)
        {
            try
            {
                IEnumerable<TrackedData> datas = context.TrackedData
                    .Where(s => s.TrackingId == request.trackingId)
                    .ToList();
                foreach (var data in datas)
                {
                    context.TrackedData.Remove(data);
                }
                context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }

        public bool deleteTrackingInfor(int trackingId)
        {
            try
            {
                IEnumerable<TrackingInfor> datas = context.TrackingInfor
                    .Where(s => s.TrackingId == trackingId)
                    .Where(s => s.IsRemoved == false)
                    .ToList();
                foreach (var data in datas)
                {
                    data.IsRemoved = true;
                }
                context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }

        public IEnumerable<TrackingDataResponse> getData(GetDataRequest request)
        {
            try
            {
                IEnumerable<TrackedData> data = context.TrackedData
                    .Where(s => s.TrackingId == request.trackingId)
                    .ToList();
                List<TrackingDataResponse> response = new List<TrackingDataResponse>();
                foreach (var data1 in data)
                {
                    response.Add(new TrackingDataResponse(data1.TrackedDataId, data1.TrackingId, data1.Data));
                }
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        public bool updateTrackingInfor(UpdateTrackingInforRequest request)
        {
            
            try
            {
                TrackingInfor info = context.TrackingInfor
                    .Where(s => s.TrackingId == request.trackingId).FirstOrDefault();
                if (info != null)
                {
                    info.WebId = request.webId;
                    info.TrackingUrl = request.trackingUrl;
                    info.TrackingType = request.trackingType;
                    context.SaveChangesAsync();
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }
    }
}
