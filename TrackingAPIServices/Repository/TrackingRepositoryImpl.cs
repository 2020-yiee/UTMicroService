using HeatMapAPIServices.Controllers;
using HeatMapAPIServices.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using StatisticAPIService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingAPIServices.EFModels;
using TrackingAPIServices.Models;

namespace HeatMapAPIServices.Repository
{
    public class TrackingRepositoryImpl : ITrackingRepository
    {
        private readonly DBUTContext context = new DBUTContext();

        private List<int> EVENT_TYPE_LIST = new List<int>();
        private List<string> TYPEURL = new List<string>();

        public TrackingRepositoryImpl()
        {
            EVENT_TYPE_LIST.Add(2);
            EVENT_TYPE_LIST.Add(1);
            EVENT_TYPE_LIST.Add(0);
            TYPEURL.Add("match");
            TYPEURL.Add("start-with");
            TYPEURL.Add("end-with");
            TYPEURL.Add("contain");
            TYPEURL.Add("regex");
        }

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

        public bool createTrackedHeatmapData(SaveDataRequest request)
        {
            try
            {
                TrackedHeatmapData trackedData = new TrackedHeatmapData();
                trackedData.TrackingUrl = request.trackingUrl;
                trackedData.WebId = request.webID;
                Website website = context.Website.Where(s => s.WebId == request.webID).FirstOrDefault();
                website.Verified = true;
                trackedData.Data = request.data;
                trackedData.EventType = request.eventType;
                var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                trackedData.CreatedAt = (long)timeSpan.TotalSeconds;
                trackedData.SessionId = request.sessionID;
                trackedData.ScreenWidth = request.screenWidth;
                trackedData.ScreenHeight = request.screenHeight;

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

        private bool checkTrackingHeatmapInfoExisted(int webId, string trackingUrl, string name)
        {
            List<string> listTrackingUrl = context.TrackingHeatmapInfo.Where(s => s.WebId == webId)
                .ToList().Select(s => s.TrackingUrl).ToList();
            List<string> listName = context.TrackingHeatmapInfo.Where(s => s.WebId == webId)
                .ToList().Select(s => s.Name).ToList();

            if (listTrackingUrl.Contains(trackingUrl) == true || listName.Contains(name) == true) return false;
            Website web = context.Website.Where(s => s.WebId == webId).FirstOrDefault();
            if (!trackingUrl.Contains(web.DomainUrl)) return true;
            return false;

        }
        private bool checkTrackingHeatmapInfoExisted1(int webId, string name)
        {
            List<string> listTrackingUrl = context.TrackingHeatmapInfo.Where(s => s.WebId == webId)
                .ToList().Select(s => s.TrackingUrl).ToList();
            List<string> listName = context.TrackingHeatmapInfo.Where(s => s.WebId == webId)
                .ToList().Select(s => s.Name).ToList();

            if (listName.Contains(name) == true) return false;
            Website web = context.Website.Where(s => s.WebId == webId).FirstOrDefault();
            return false;

        }


        public IActionResult getCheckingHeatmapInfo(int websiteId, int userId)
        {
            try
            {
                if (!checkAuthencation(websiteId, userId)) return new UnauthorizedResult();
                IEnumerable<TrackingHeatmapInfo> trackingInfo = context.TrackingHeatmapInfo
                    .Where(s => s.WebId == websiteId)
                    .Where(s => s.Removed == false)
                    .ToList();
                return new OkObjectResult(trackingInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return new BadRequestResult();
            }
        }

        private Boolean checkAuthencation(int websiteId, int userId)
        {
            List<int> orgIds = context.Access.Where(s => s.UserId == userId).Select(s => s.OrganizationId).ToList();
            if (orgIds == null || orgIds.Count == 0) return false;
            Website website = context.Website.FirstOrDefault(s => s.WebId == websiteId);
            if (website == null) return false;
            if (!orgIds.Contains(website.OrganizationId)) return false;
            if (context.Access.Where(s => s.OrganizationId == website.OrganizationId &&
             s.UserId == userId).Select(s => s.Role).FirstOrDefault() != 1) return false;
            return true;
        }

        public IActionResult createHeatmapTrackingInfor(CreateTrackingHeatmapInforRequest request, int userId)
        {
            if (!checkAuthencation(request.webID, userId)) return new BadRequestResult();
            if (checkTrackingHeatmapInfoExisted(request.webID, request.trackingUrl, request.name)) return new BadRequestResult();
            if (!checkDomainUrl(request.webID, request.trackingUrl)) return null;
            if (!TYPEURL.Contains(request.typeUrl)) return new BadRequestResult();
            TrackingHeatmapInfo info = new TrackingHeatmapInfo();
            info.WebId = request.webID;
            info.Name = request.name;
            info.TrackingUrl = request.trackingUrl;
            info.Removed = false;
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            info.CreatedAt = (long)timeSpan.TotalSeconds;
            info.TypeUrl = request.typeUrl;
            try
            {
                context.TrackingHeatmapInfo.Add(info);
                context.SaveChanges();
                var client = new RestClient("https://browser-service.herokuapp.com/capture/" + info.WebId + "/" + info.TrackingHeatmapInfoId + "/" + info.TrackingUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);
                var requests = new RestRequest();
                requests.AddHeader("Content-Type", "application/json");
                var response = client.Post(requests);
                var content = response.Content;
                CaptureResponse captureResponse = JsonConvert.DeserializeObject<CaptureResponse>(content);
                info.SmImageUrl = captureResponse.smImageUrl;
                info.MdImageUrl = captureResponse.mdImageUrl;
                info.LgImageUrl = captureResponse.lgImageUrl;
                context.SaveChanges();
                return new OkObjectResult(info);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return new BadRequestResult();
            }
        }

        public TrackingHeatmapInfo updateTrackingHeatmapInfor(UpdateTrackingHeatmapInforRequest request, int userId)
        {
            TrackingHeatmapInfo trackingHeatmapInfo = context.TrackingHeatmapInfo.Where(s => s.TrackingHeatmapInfoId == request.trackingHeatmapInfoID).FirstOrDefault();
            if (!checkTrackingHeatmapInfoExisted1(trackingHeatmapInfo.WebId, request.newName)) return null;
            if (!checkAuthencation(trackingHeatmapInfo.WebId, userId)) return null;
            try
            {
                TrackingHeatmapInfo info = context.TrackingHeatmapInfo
                    .Where(s => s.TrackingHeatmapInfoId == request.trackingHeatmapInfoID).FirstOrDefault();
                if (info != null)
                {
                    info.Name = request.newName;
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

        private bool checkDomainUrl(int webId, string url)
        {
            try
            {
                string domainUrl = context.Website.Where(s => s.WebId == webId).Select(s => s.DomainUrl).FirstOrDefault();
                if (!url.Contains(domainUrl)) return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR " + ex.Message);
                return false;
                throw;
            }
            return true;
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
                    if (!TYPEURL.Contains(item.typeUrl)) return false;
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

        public IActionResult createFunnelTrackingInfo(CreateTrackingFunnelInforRequest request, int userId)
        {
            if (!checkAuthencation(request.webID, userId)) return new BadRequestResult();
            if (checkTrackingFunnelInfoExisted(request.webID, request.steps.ToString(), request.name)) return new BadRequestResult();
            if (!checkStepDomainUrl(request.webID, request.steps.ToString())) return new BadRequestResult();
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
                return new OkObjectResult(info);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return new BadRequestResult();
            }
        }

        public object udpateFunnelTrackingInfo(udpateTrackingStepInfoRequest request, int userId)
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
        public bool createTrackedFunnelData(SaveFunnelDataRequest request)
        {
            try
            {
                TrackedFunnelData datac = checkTrackedFunnelDataExisted(request.sessionID);
                if (datac != null)
                {
                    datac.TrackedSteps = request.trackedSteps;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    datac = new TrackedFunnelData();
                    datac.SessionId = request.sessionID;
                    datac.WebId = request.webID;
                    Website website = context.Website.Where(s => s.WebId == request.webID).FirstOrDefault();
                    website.Verified = true;
                    datac.TrackedSteps = request.trackedSteps;
                    var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                    datac.CreatedAt = (long)timeSpan.TotalSeconds;
                    context.TrackedFunnelData.Add(datac);
                    context.SaveChanges();
                    return true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        public object getStatisticHeatMap(int webID, int trackingInfoID, int from, int to, int device, int userId)
        {
            GetStatisicHeatMap response = new GetStatisicHeatMap();
            try
            {
                if (!checkAuthencation(webID, userId)) return null;
                TrackingHeatmapInfo trackingHeatmapInfo = context.TrackingHeatmapInfo.Where(s => s.WebId == webID)
                .Where(s => s.TrackingHeatmapInfoId == trackingInfoID)
                .FirstOrDefault();
                switch (device)
                {
                    case 0: response.imageUrl = trackingHeatmapInfo.SmImageUrl; break;
                    case 1: response.imageUrl = trackingHeatmapInfo.MdImageUrl; break;
                    case 2: response.imageUrl = trackingHeatmapInfo.LgImageUrl; break;
                }
                foreach (var eventType in EVENT_TYPE_LIST)
                {
                    List<TrackedHeatmapData> trackedHeatmapDatas = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                                .Where(s => s.TrackingUrl == trackingHeatmapInfo.TrackingUrl)
                                .Where(s => s.CreatedAt >= from)
                                .Where(s => s.CreatedAt <= to)
                                .Where(s => s.EventType == eventType).ToList();
                    switch (device)
                    {
                        case 0:
                            List<int> trackedHeatmapDataIds = trackedHeatmapDatas
                                .Where(s => s.ScreenWidth < 540)
                                .Select(s => s.TrackedHeatmapDataId).ToList();
                            List<StatisticHeatmap> statisticHeatmaps = context.StatisticHeatmap
                                .Where(s => trackedHeatmapDataIds.Contains(s.TrackedHeatmapDataId) == true)
                            .ToList();
                            List<StatisticData> statisticDatas = new List<StatisticData>();
                            foreach (var item in statisticHeatmaps)
                            {
                                statisticDatas.AddRange(JsonConvert.DeserializeObject<List<StatisticData>>(item.StatisticData));
                            }
                            if (eventType == 0) response.click = JsonConvert.SerializeObject(statisticDatas);
                            if (eventType == 1) response.hover = JsonConvert.SerializeObject(statisticDatas); break;
                        case 1:
                            trackedHeatmapDataIds = trackedHeatmapDatas
                           .Where(s => s.ScreenWidth < 768 && s.ScreenWidth >= 540)
                           .Select(s => s.TrackedHeatmapDataId).ToList();
                            statisticHeatmaps = context.StatisticHeatmap
                                .Where(s => trackedHeatmapDataIds.Contains(s.TrackedHeatmapDataId) == true)
                            .ToList();
                            statisticDatas = new List<StatisticData>();
                            foreach (var item in statisticHeatmaps)
                            {
                                statisticDatas.AddRange(JsonConvert.DeserializeObject<List<StatisticData>>(item.StatisticData));
                            }
                            if (eventType == 0) response.click = JsonConvert.SerializeObject(statisticDatas);
                            if (eventType == 1) response.hover = JsonConvert.SerializeObject(statisticDatas); break;
                        case 2:
                            trackedHeatmapDataIds = trackedHeatmapDatas
                        .Where(s => s.ScreenWidth >= 768)
                        .Select(s => s.TrackedHeatmapDataId).ToList();
                            statisticHeatmaps = context.StatisticHeatmap
                                .Where(s => trackedHeatmapDataIds.Contains(s.TrackedHeatmapDataId) == true)
                            .ToList();
                            statisticDatas = new List<StatisticData>();
                            foreach (var item in statisticHeatmaps)
                            {
                                statisticDatas.AddRange(JsonConvert.DeserializeObject<List<StatisticData>>(item.StatisticData));
                            }
                            if (eventType == 0) response.click = JsonConvert.SerializeObject(statisticDatas);
                            if (eventType == 1) response.hover = JsonConvert.SerializeObject(statisticDatas); break;
                    }
                }

                //scroll
                List<long?> listScreenHeight = context.TrackedHeatmapData.Select(s => s.ScreenHeight).Distinct().ToList();
                List<ScrollResponse> scrolls = new List<ScrollResponse>();

                foreach (var item in listScreenHeight)
                {
                    long documentHeight = 0;
                    List<double> scrollData = new List<double>();
                    List<int> trackedDataIDs = new List<int>();
                    switch (device)
                    {
                        case 0:
                            trackedDataIDs = context.TrackedHeatmapData
                            .Where(s => s.ScreenHeight == item && s.EventType == 2 && s.ScreenWidth < 540)
                            .Select(s => s.TrackedHeatmapDataId).ToList();
                            break;
                        case 1:
                            trackedDataIDs = context.TrackedHeatmapData
                                .Where(s => s.ScreenHeight == item && s.EventType == 2 && s.ScreenWidth < 768 && s.ScreenWidth >= 540)
                                .Select(s => s.TrackedHeatmapDataId).ToList();
                            break;
                        case 2:
                            trackedDataIDs = context.TrackedHeatmapData
                            .Where(s => s.ScreenHeight == item && s.EventType == 2 && s.ScreenWidth >= 768)
                            .Select(s => s.TrackedHeatmapDataId).ToList();
                            break;
                    }
                    foreach (var id in trackedDataIDs)
                    {
                        StatisticHeatmap statisticHeatmap = context.StatisticHeatmap.Where(s => s.TrackedHeatmapDataId == id).FirstOrDefault();
                        StatisticScrollData statisticScrollData = JsonConvert.DeserializeObject<StatisticScrollData>(statisticHeatmap.StatisticData);
                        documentHeight = statisticScrollData.documentHeight;
                        scrollData.AddRange(statisticScrollData.scroll);
                    }

                    if (trackedDataIDs.Count != 0)
                    {
                        ScrollResponse scrollResponse = new ScrollResponse();
                        scrollResponse.height = item.GetValueOrDefault();
                        scrollResponse.documentHeight = documentHeight;
                        scrollResponse.positions = JsonConvert.SerializeObject(scrollData);
                        scrolls.Add(scrollResponse);
                    }
                }

                response.scroll = JsonConvert.SerializeObject(scrolls);




                List<TrackedHeatmapData> datas = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                        .Where(s => s.TrackingUrl == trackingHeatmapInfo.TrackingUrl && s.CreatedAt >= from && s.CreatedAt <= to)
                        .ToList();

                List<DateTime> times = new List<DateTime>();
                foreach (var item in datas)
                {
                    DateTime time = convertTimeStamp(item.CreatedAt);
                    int day = time.Day;
                    int month = time.Month;
                    int year = time.Year;
                    DateTime timetemp = new DateTime(year, month, day);
                    times.Add(timetemp);
                }
                times = times.Distinct().ToList();

                List<VisitResponse> visitResponses = new List<VisitResponse>();
                foreach (DateTime time in times)
                {
                    VisitResponse visit = new VisitResponse();
                    Console.WriteLine(time);
                    visit.x = ((DateTimeOffset)time).ToUnixTimeSeconds();
                    var timeList = new List<TrackedHeatmapData>();


                    int currentDay = time.Day;
                    int currentMonth = time.Month;
                    int currentYear = time.Year;
                    foreach (var item in datas)
                    {
                        DateTime t = convertTimeStamp(item.CreatedAt);
                        int day = t.Day;
                        int month = t.Month;
                        int year = t.Year;

                        if (day == currentDay && currentMonth == month && currentYear == year)
                        {
                            timeList.Add(item);
                        }
                    }

                    visit.y = timeList.Select(item => item.SessionId).Distinct().ToList().Count;

                    visitResponses.Add(visit);
                }
                response.visit = JsonConvert.SerializeObject(visitResponses);





                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }

        private DateTime convertTimeStamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public Object getStatisticFunnel(int webID, int trackingFunnelInfoID, int from, int to, int userId)
        {
            try
            {
                if (!checkAuthencation(webID, userId)) return null;
                TrackingFunnelInfo trackingFunnelInfo = context.TrackingFunnelInfo.FirstOrDefault(s => s.TrackingFunnelInfoId == trackingFunnelInfoID);
                List<Step> trackingInfoFunnelSteps = JsonConvert.DeserializeObject<List<Step>>(trackingFunnelInfo.Steps);
                List<int> trackedFunnelDataIDs = context.TrackedFunnelData
                    .Where(s => s.WebId == webID && from <= s.CreatedAt && s.CreatedAt <= to)
                    .Select(s => s.TrackedFunnelDataId)
                    .ToList();
                List<StatisticFunnel> statisticFunnels = context.StatisticFunnel.Where(s => trackedFunnelDataIDs.Contains(s.TrackedFunnelDataId)).ToList();
                List<StatisticFunnelResponse> statisticFunnelResponses = new List<StatisticFunnelResponse>();
                for (int i = 1; i <= trackingInfoFunnelSteps.Count; i++)
                {
                    List<Step> steps = trackingInfoFunnelSteps.GetRange(0, i);
                    List<string> stepsUrl = steps.Select(s => s.stepUrl).ToList();
                    int count = 0;
                    foreach (var item in statisticFunnels)
                    {
                        List<string> statisticData = JsonConvert.DeserializeObject<List<string>>(item.StatisticData);

                        for (int j = 0; j < statisticData.Count; j++)
                        {
                            int downcount = steps.Count;
                            var matchStep = steps[0];
                            if (statisticData[j].Equals(matchStep.stepUrl))
                            {
                                downcount -= 1;
                                if (downcount == 0) count += 1;
                                int index = j;
                                while (downcount > 0)
                                {
                                    index += 1;
                                    if (index >= statisticData.Count) break;
                                    matchStep = steps[steps.IndexOf(matchStep) + 1];
                                    string nextStep = statisticData[index];
                                    if (nextStep.Equals(matchStep.stepUrl))
                                    {
                                        downcount -= 1;
                                        if (downcount == 0) count += 1;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                    }

                    StatisticFunnelResponse statisticFunnelResponse =
                        new StatisticFunnelResponse(trackingInfoFunnelSteps[i - 1].name, trackingInfoFunnelSteps[i - 1].stepUrl
                        , trackingInfoFunnelSteps[i - 1].typeUrl, count);
                    statisticFunnelResponses.Add(statisticFunnelResponse);
                }
                return statisticFunnelResponses;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n==========================================================================\nERROR: " + ex.Message);
                return null;
            }
        }

        public IActionResult udpateNameFunnelTrackingInfo(udpateTrackingNameInfoRequest request, int v)
        {
            TrackingFunnelInfo trackingFunnelInfo = context.TrackingFunnelInfo.Where(s => s.TrackingFunnelInfoId == request.trackingFunnelInfoID)
                .FirstOrDefault();
            if (trackingFunnelInfo == null) return new NotFoundResult();
            Website website = context.Website.Where(s => s.WebId == trackingFunnelInfo.WebId).FirstOrDefault();
            Access access = context.Access.Where(s => s.OrganizationId == website.OrganizationId
            && s.UserId == v).FirstOrDefault();
            if (access == null) return new BadRequestResult();
            if (access.Role != 1) return new UnauthorizedResult();
            List<string> names = context.TrackingFunnelInfo.Where(s => s.WebId == website.WebId)
                .Select(s => s.Name).ToList();
            if (names.Contains(request.newName)) return new BadRequestResult();
            trackingFunnelInfo.Name = request.newName;
            context.SaveChanges();
            return new OkObjectResult(trackingFunnelInfo);
        }
    }

}
