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

        public TrackingInfor checkTrackingType(checkingRequest request)
        {
            try
            {
                return context.TrackingInfor
                    .Where(s => s.WebId == request.WebId)
                    .Where(s => s.TrackingUrl == request.TrackingUrl)
                    .Where(s => s.IsRemoved == false)
                    .FirstOrDefault();
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
                trackedData.TrackingId = data.TrackingId;
                trackedData.Data = data.Data;
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
            info.WebId = request.WebId;
            info.TrackingUrl = request.TrackingUrl;
            info.TrackingType = request.TrackingType;
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
                    .Where(s => s.TrackingId == request.TrackingId)
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

        public IEnumerable<TrackedData> getData(GetDataRequest request)
        {
            try
            {
                IEnumerable<TrackedData> data = context.TrackedData
                    .Where(s => s.TrackingId == request.TrackingId)
                    .ToList();
                return data;
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
                    .Where(s => s.TrackingId == request.TrackingId).FirstOrDefault();
                if (info != null)
                {
                    info.WebId = request.WebId;
                    info.TrackingUrl = request.TrackingUrl;
                    info.TrackingType = request.TrackingType;
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
