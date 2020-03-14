using HeatMapAPIServices.Controllers;
using HeatMapAPIServices.EFModels;
using HeatMapAPIServices.Models;
using Newtonsoft.Json;
using StatisticAPIService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Repository
{
    public class TrackingRepositoryImpl : ITrackingRepository
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
                var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                trackedData.CreatedAt = (long)timeSpan.TotalSeconds;
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
        //----------------------------------statistic heatmap------------------------------------
        public List<StatisticHeatmap> getstatisticHeatmapData(int trackedHeatmapInfoID)
        {
            try
            {
                return context.StatisticHeatmap.Where(s => s.TrackedHeatmapDataId == trackedHeatmapInfoID).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
                throw;
            }
        }

        //----------------------------------tracking heatmap info--------------------------------

        private bool checkTrackingHeatmapInfoExisted(int webId,string trackingUrl, string name)
        {
            List<string> listTrackingUrl = context.TrackingHeatmapInfo.Where(s => s.WebId == webId)
                .ToList().Select(s => s.TrackingUrl).ToList();
            List<string> listName = context.TrackingHeatmapInfo.Where(s => s.WebId == webId)
                .ToList().Select(s => s.Name).ToList();

            if (listTrackingUrl.Contains(trackingUrl) == true || listName.Contains(name) == true) return false;
            Website web = context.Website.Where(s => s.WebId == webId).FirstOrDefault();
            if (!trackingUrl.Contains(web.DomainUrl)) return false;
            return true;

        }


        public IEnumerable<TrackingHeatmapInfo> getCheckingHeatmapInfo(int websiteId, int userId)
        {
            try
            {
                if (!checkAuthencation(websiteId, userId)) return null;
                IEnumerable<TrackingHeatmapInfo> trackingInfo = context.TrackingHeatmapInfo
                    .Where(s => s.WebId == websiteId)
                    .Where(s => s.Removed == false)
                    .ToList();
                return trackingInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        private Boolean checkAuthencation(int websiteId, int userId)
        {
            List<int> orgIds = context.Access.Where(s => s.UserId == userId).Select(s => s.OrganizationId).ToList();
            if (orgIds == null || orgIds.Count == 0) return false;
            Website website = context.Website.FirstOrDefault(s => s.WebId == websiteId);
            if (website == null) return false;
            return orgIds.Contains(website.OrganizationId);
        }

        public TrackingHeatmapInfo createHeatmapTrackingInfor(CreateTrackingHeatmapInforRequest request, int userId)
        {
            if (!checkAuthencation(request.webID, userId)) return null;
            if (checkTrackingHeatmapInfoExisted(request.webID, request.trackingUrl, request.name)) return null;
            TrackingHeatmapInfo info = new TrackingHeatmapInfo();
            info.WebId = request.webID;
            info.Name = request.name;
            info.TrackingUrl = request.trackingUrl;
            info.Removed = false;
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            info.CreatedAt =(long) timeSpan.TotalSeconds;
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

        public TrackingHeatmapInfo updateTrackingHeatmapInfor(UpdateTrackingHeatmapInforRequest request, int userId)
        {
            if (!checkTrackingHeatmapInfoExisted(request.webID, request.trackingUrl, request.name)) return null;
            if (!checkAuthencation(request.webID, userId)) return null;
            try
            {
                TrackingHeatmapInfo info = context.TrackingHeatmapInfo
                    .Where(s => s.TrackingHeatmapInfoId == request.trackingHeatmapInfoID).FirstOrDefault();
                if (info != null)
                {
                    info.WebId = request.webID;
                    info.Name = request.name;
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
        public bool deleteTrackingHeatmapInfor(int trackingHeatmapInfoId, int userId)
        {
            TrackingHeatmapInfo thi = context.TrackingHeatmapInfo.Where(s => s.TrackingHeatmapInfoId == trackingHeatmapInfoId).FirstOrDefault();
            if (thi == null) return false;
            if (!checkAuthencation(thi.WebId, userId)) return false;
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

        //----------------------------tracking funnel info-----------------------------------

        private bool checkTrackingFunnelInfoExisted(int webId, string steps, string name)
        {
            List<string> listSteps = context.TrackingFunnelInfo.Where(s => s.WebId == webId)
                .ToList().Select(s => s.Steps).ToList();
            List<string> listName = context.TrackingFunnelInfo.Where(s => s.WebId == webId)
                .ToList().Select(s => s.Name).ToList();

            if (listSteps.Contains(steps) == true || listName.Contains(name) == true) return true;

            return false;
        }

        private bool checkStepDomainUrl(int webId, string steps)
        {
            try
            {
                string domainUrl = context.Website.Where(s => s.WebId == webId).Select(s => s.DomainUrl).FirstOrDefault();
                List<Step> deserializedSteps = JsonConvert.DeserializeObject<List<Step>>(steps);
                foreach (Step item in deserializedSteps)
                {
                    if (!item.stepUrl.Contains(domainUrl)) return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR " + ex.Message);
                return false;
                throw;
            }
            return true;
        }
        public IEnumerable<TrackingFunnelInfo> getCheckingFunnelInfo(int websiteId, int userId)
        {
            try
            {
                if (!checkAuthencation(websiteId, userId)) return null;
                IEnumerable<TrackingFunnelInfo> trackingInfo = context.TrackingFunnelInfo
                    .Where(s => s.WebId == websiteId)
                    .Where(s => s.Removed == false)
                    .ToList();
                return trackingInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        public object createFunnelTrackingInfo(CreateTrackingFunnelInforRequest request, int userId)
        {
            if (!checkAuthencation(request.webID, userId)) return null;
            if (checkTrackingFunnelInfoExisted(request.webID,request.steps.ToString(),request.name)) return null;
            if (!checkStepDomainUrl(request.webID, request.steps.ToString())) return null;
            TrackingFunnelInfo info = new TrackingFunnelInfo();
            info.WebId = request.webID;
            info.Name = request.name;
            info.Steps = request.steps.ToString();
            info.Removed = false;
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            info.CreatedAt = (long)timeSpan.TotalSeconds;
            try
            {
                context.TrackingFunnelInfo.Add(info);
                context.SaveChanges();
                return info;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        public object udpateFunnelTrackingInfo(udpateTrackingInfoRequest request, int userId)
        {
            try
            {
                if (!checkAuthencation(request.webID, userId)) return null;
                if (!checkStepDomainUrl(request.webID, request.steps.ToString())) return null;
                TrackingFunnelInfo info = context.TrackingFunnelInfo
                    .Where(s => s.TrackingFunnelInfoId == request.trackingFunnelInfoID)
                    .FirstOrDefault();
                if (info == null) return null;
                info.Steps = request.steps.ToString();
                context.SaveChanges();
                return info;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR " + ex.Message);
                return null;
            }
        }

        public bool deleteTrackingFunnelInfo(int trackingId, int userId)
        {
            TrackingFunnelInfo thi = context.TrackingFunnelInfo.Where(s => s.TrackingFunnelInfoId == trackingId).FirstOrDefault();
            if (thi == null) return false;
            if (!checkAuthencation(thi.WebId, userId)) return false;
            try
            {
                List<TrackingFunnelInfo> datas = context.TrackingFunnelInfo
                    .Where(s => s.TrackingFunnelInfoId == trackingId)
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


        //===========================tracked funnel data ==============================
        
        private TrackedFunnelData checkTrackedFunnelDataExisted(string sessionId)
        {
            return context.TrackedFunnelData.Where(s => s.SessionId == sessionId).FirstOrDefault();
        }
        public bool createTrackedFunnelData(SaveFunnelDataRequest data)
        {
            try
            {
                TrackedFunnelData datac = checkTrackedFunnelDataExisted(data.sessionID);
                if (datac != null)
                {
                    datac.TrackedSteps = data.trackedSteps;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    datac = new TrackedFunnelData();
                    datac.SessionId = data.sessionID;
                    datac.WebId = data.webID;
                    datac.TrackedSteps = data.trackedSteps;
                    var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                    datac.CreatedAt = (long)timeSpan.TotalSeconds;
                    context.TrackedFunnelData.Add(datac);
                    context.SaveChanges();
                    return true;
                }

            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public List<TrackedFunnelData> getTrackedFunnelData(int webID)
        {
            try
            {
                IEnumerable<TrackedFunnelData> data = context.TrackedFunnelData
                    .Where(s => s.WebId == webID)
                    .ToList();

                return data.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        //=======================statistic funnel ========================================

        public List<StatisticFunnel> getstatisticFunnelData(int trackedFunnelInfoID)
        {
            try
            {
                return context.StatisticFunnel.Where(s => s.TrackedFunnelDataId == trackedFunnelInfoID).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
                throw;
            }
        }

       
    }
}
