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

        public IEnumerable<TrackedHeatmapData> getTrackedHeatmapData(string trackingUrl, int type)
        {
            try
            {
                IEnumerable<TrackedHeatmapData> data = context.TrackedHeatmapData
                    .Where(s => s.TrackingUrl == trackingUrl)
                    .Where(s => s.EventType == type)
                    .ToList();

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        public bool createTrackedHeatmapData(SaveDataRequest data)
        {
            try
            {
                TrackedHeatmapData trackedData = new TrackedHeatmapData();
                trackedData.TrackingUrl = data.trackingUrl;
                trackedData.WebId = data.webID;
                trackedData.Data = data.data;
                trackedData.EventType = data.eventType;
                context.TrackedHeatmapData.Add(trackedData);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        

        public bool deleteTrackedHeatmapData(DeleteDataRequest request)
        {
            try
            {
                List<TrackedHeatmapData> datas = context.TrackedHeatmapData
                    .Where(s => s.TrackingUrl == request.trackingUrl)
                    .ToList();
                if (datas == null || datas.Count == 0) return false;
                foreach (var data in datas)
                {
                    context.TrackedHeatmapData.Remove(data);
                }
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }

        
       //------------------------------------------------------------------

        public IEnumerable<TrackingInforResponse> getCheckingHeatmapInfo( int websiteId)
        {
            try
            {
                IEnumerable<TrackingInforResponse> trackingInfo = context.TrackingHeatmapInfo
                    .Where(s => s.WebId == websiteId)
                    .Where(s => s.Removed == false)
                    .ToList().Select(x => new TrackingInforResponse(x.TrackingHeatmapInfoId,x.WebId,x.TrackingUrl,x.Removed));

                return trackingInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        public TrackingHeatmapInfo createHeatmapTrackingInfor(CreateTrackingInforRequest request)
        {
            TrackingHeatmapInfo info = new TrackingHeatmapInfo();
            info.WebId = request.webID;
            info.TrackingUrl = request.trackingUrl;
            info.Removed = false;
            try
            {
                context.TrackingHeatmapInfo.Add(info);
                context.SaveChanges();
                return info;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        public TrackingHeatmapInfo updateTrackingHeatmapInfor(UpdateTrackingHeatmapInforRequest request)
        {
            
            try
            {
                TrackingHeatmapInfo info = context.TrackingHeatmapInfo
                    .Where(s => s.TrackingHeatmapInfoId == request.trackingHeatmapInfoID).FirstOrDefault();
                if (info != null)
                {
                    info.WebId = request.webID;
                    info.TrackingUrl = request.trackingUrl;
                    context.SaveChanges();
                    return info;
                }
                else return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }
        public bool deleteTrackingHeatmapInfor(int trackingHeatmapInfoId)
        {
            try
            {
                List<TrackingHeatmapInfo> datas = context.TrackingHeatmapInfo
                    .Where(s => s.TrackingHeatmapInfoId == trackingHeatmapInfoId)
                    .Where(s => s.Removed == false)
                    .ToList();
                if (datas == null || datas.Count == 0) return false;
                foreach (var data in datas)
                {
                    data.Removed = true;
                }
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }
    }
}
