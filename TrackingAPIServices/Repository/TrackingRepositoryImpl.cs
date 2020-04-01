using HeatMapAPIServices.Controllers;
using HeatMapAPIServices.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using StatisticAPIService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                Website website1 = context.Website.Where(s => s.WebId == request.webID).FirstOrDefault();
                if (website1 == null || website1.Removed == true) return false;
                User user = context.User.Where(s => s.UserId == website1.AuthorId).FirstOrDefault();
                if (user == null || user.Actived == false) return false;
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

        public IActionResult createHeatmapTrackingInfo(CreateTrackingHeatmapInforRequest request, int userId)
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
                var client = new RestClient("https://browser-service.herokuapp.com/capture/" + info.WebId + "/" + info.TrackingHeatmapInfoId + "/" + request.captureUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);
                var requests = new RestRequest();
                requests.AddHeader("Content-Type", "application/json");
                requests.Timeout = 20000;
                string content = null;
                try
                {
                    var response = client.Post(requests);
                    content = response.Content;
                }
                catch (Exception)
                {
                    context.TrackingHeatmapInfo.Remove(info);
                    context.SaveChanges();
                    throw;
                }
                if (content == null)
                {
                    context.TrackingHeatmapInfo.Remove(info);
                    context.SaveChanges();
                }
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

        public IActionResult updateTrackingHeatmapInfo(UpdateTrackingHeatmapInforRequest request, int userId)
        {
            TrackingHeatmapInfo trackingHeatmapInfo = context.TrackingHeatmapInfo.Where(s => s.TrackingHeatmapInfoId == request.trackingHeatmapInfoID).FirstOrDefault();
            if (!checkTrackingHeatmapInfoExisted1(trackingHeatmapInfo.WebId, request.newName)) return new BadRequestResult();
            if (!checkAuthencation(trackingHeatmapInfo.WebId, userId)) return new BadRequestResult();
            try
            {
                TrackingHeatmapInfo info = context.TrackingHeatmapInfo
                    .Where(s => s.TrackingHeatmapInfoId == request.trackingHeatmapInfoID).FirstOrDefault();
                if (info != null)
                {
                    info.Name = request.newName;
                    context.SaveChanges();
                    return new OkObjectResult(info);
                }
                else return new BadRequestResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return new UnprocessableEntityResult();
            }
        }
        public bool deleteTrackingHeatmapInfo(int trackingHeatmapInfoId, int userId)
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

        private bool checkStepDomainUrl(int webId, List<Step> steps)
        {
            try
            {
                string domainUrl = context.Website.Where(s => s.WebId == webId).Select(s => s.DomainUrl).FirstOrDefault();
                List<Step> deserializedSteps = steps;
                foreach (Step item in deserializedSteps)
                {
                    if (!TYPEURL.Contains(item.typeUrl)) return false;
                    if (item.typeUrl == "match" && item.typeUrl == "start-with")
                    {
                        if (!item.stepUrl.Contains(domainUrl)) return false;
                    }
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
            if (!checkStepDomainUrl(request.webID, request.steps)) return new BadRequestResult();
            TrackingFunnelInfo info = new TrackingFunnelInfo();
            info.WebId = request.webID;
            info.Name = request.name;
            info.Steps = JsonConvert.SerializeObject(request.steps);
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

        public IActionResult udpateFunnelTrackingInfo(udpateTrackingStepInfoRequest request, int userId)
        {
            try
            {
                if (!checkAuthencation(request.webID, userId)) return new BadRequestResult();
                if (!checkStepDomainUrl(request.webID, request.steps)) return new BadRequestResult();
                udpateNameFunnelTrackingInfo(
                    new udpateTrackingNameInfoRequest(request.trackingFunnelInfoID, request.newName), userId);
                TrackingFunnelInfo info = context.TrackingFunnelInfo
                    .Where(s => s.TrackingFunnelInfoId == request.trackingFunnelInfoID)
                    .FirstOrDefault();
                if (info == null) return new BadRequestResult();
                info.Steps = JsonConvert.SerializeObject(request.steps);
                context.SaveChanges();
                return new OkObjectResult(info);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR " + ex.Message);
                return new UnprocessableEntityResult();
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
                Website website1 = context.Website.Where(s => s.WebId == request.webID).FirstOrDefault();
                if (website1 == null || website1.Removed == true) return false;
                User user = context.User.Where(s => s.UserId == website1.AuthorId).FirstOrDefault();
                if (user == null || user.Actived == false) return false;

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

        public IActionResult getStatisticHeatMap(int webID, int trackingInfoID, int from, int to, int device, int userId)
        {
            GetStatisicHeatMap response = new GetStatisicHeatMap();
            try
            {
                if (!checkAuthencation(webID, userId)) return new UnauthorizedResult();

                //click hover
                TrackingHeatmapInfo trackingHeatmapInfo = context.TrackingHeatmapInfo.Where(s => s.WebId == webID)
                .Where(s => s.TrackingHeatmapInfoId == trackingInfoID)
                .FirstOrDefault();
                if (trackingHeatmapInfo == null) return new NotFoundResult();
                if (trackingHeatmapInfo.Removed) return new NotFoundResult();
                response.name = trackingHeatmapInfo.Name;
                response.typeUrl = trackingHeatmapInfo.TypeUrl;
                response.trackingUrl = trackingHeatmapInfo.TrackingUrl;
                switch (device)
                {
                    case 0: response.imageUrl = trackingHeatmapInfo.SmImageUrl; break;
                    case 1: response.imageUrl = trackingHeatmapInfo.MdImageUrl; break;
                    case 2: response.imageUrl = trackingHeatmapInfo.LgImageUrl; break;
                }
                foreach (var eventType in EVENT_TYPE_LIST)
                {
                    List<TrackedHeatmapData> trackedHeatmapDatas = new List<TrackedHeatmapData>();
                    switch (trackingHeatmapInfo.TypeUrl)
                    {
                        case "match":
                            trackedHeatmapDatas = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                               .Where(s => s.TrackingUrl == trackingHeatmapInfo.TrackingUrl)
                               .Where(s => s.CreatedAt >= from)
                               .Where(s => s.CreatedAt <= to)
                               .Where(s => s.EventType == eventType).ToList(); break;
                        case "contain":
                            trackedHeatmapDatas = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                                .Where(s => s.TrackingUrl.Contains(trackingHeatmapInfo.TrackingUrl))
                                .Where(s => s.CreatedAt >= from)
                                .Where(s => s.CreatedAt <= to)
                                .Where(s => s.EventType == eventType).ToList(); break;
                        case "start-with":
                            trackedHeatmapDatas = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                                .Where(s => s.TrackingUrl.LastIndexOf(trackingHeatmapInfo.TrackingUrl) == 0)
                                .Where(s => s.CreatedAt >= from)
                                .Where(s => s.CreatedAt <= to)
                                .Where(s => s.EventType == eventType).ToList(); break;
                        case "end-with":
                            trackedHeatmapDatas = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                                .Where(s => (s.TrackingUrl.LastIndexOf(trackingHeatmapInfo.TrackingUrl) + trackingHeatmapInfo.TrackingUrl.Length)
                                == s.TrackingUrl.Length)
                                .Where(s => s.CreatedAt >= from)
                                .Where(s => s.CreatedAt <= to)
                                .Where(s => s.EventType == eventType).ToList(); break;
                        case "regex":
                            List<TrackedHeatmapData> trackedHeatmapData = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                                .Where(s => s.CreatedAt >= from)
                                .Where(s => s.CreatedAt <= to)
                                .Where(s => s.EventType == eventType).ToList();
                            Regex reg = new Regex(trackingHeatmapInfo.TrackingUrl);
                            foreach (var item in trackedHeatmapData)
                            {
                                if (reg.IsMatch(item.TrackingUrl)) trackedHeatmapDatas.Add(item);
                            }
                            break;
                    }
                    if (trackedHeatmapDatas == null || trackedHeatmapDatas.Count == 0) return new NotFoundResult();
                    List<StatisticHeatmap> statisticHeatmaps = new List<StatisticHeatmap>();
                    switch (device)
                    {
                        case 0:
                            List<int> trackedHeatmapDataIds = trackedHeatmapDatas
                                .Where(s => s.ScreenWidth < 540)
                                .Select(s => s.TrackedHeatmapDataId).ToList();
                            statisticHeatmaps = context.StatisticHeatmap
                                .Where(s => trackedHeatmapDataIds.Contains(s.TrackedHeatmapDataId) == true)
                            .ToList(); break;
                        case 1:
                            trackedHeatmapDataIds = trackedHeatmapDatas
                           .Where(s => s.ScreenWidth < 768 && s.ScreenWidth >= 540)
                           .Select(s => s.TrackedHeatmapDataId).ToList();
                            statisticHeatmaps = context.StatisticHeatmap
                                .Where(s => trackedHeatmapDataIds.Contains(s.TrackedHeatmapDataId) == true)
                            .ToList();
                            break;
                        case 2:
                            trackedHeatmapDataIds = trackedHeatmapDatas
                                .Where(s => s.ScreenWidth >= 768)
                                .Select(s => s.TrackedHeatmapDataId).ToList();
                            statisticHeatmaps = context.StatisticHeatmap
                                .Where(s => trackedHeatmapDataIds.Contains(s.TrackedHeatmapDataId) == true)
                            .ToList();
                            break;
                    }
                    List<StatisticData> statisticDatas = new List<StatisticData>();
                    foreach (var item in statisticHeatmaps)
                    {
                        try
                        {
                            statisticDatas.AddRange(JsonConvert.DeserializeObject<List<StatisticData>>(item.StatisticData));
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (eventType == 0) response.click = JsonConvert.SerializeObject(statisticDatas);
                    if (eventType == 1) response.hover = JsonConvert.SerializeObject(statisticDatas);
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
                        if (statisticHeatmap != null)
                        {
                            StatisticScrollData statisticScrollData = JsonConvert.DeserializeObject<StatisticScrollData>(statisticHeatmap.StatisticData);
                            documentHeight = statisticScrollData.documentHeight;
                            scrollData.AddRange(statisticScrollData.scroll);
                        }
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



                //count
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





                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new UnprocessableEntityResult();

            }
        }

        private DateTime convertTimeStamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public IActionResult getStatisticFunnel(int webID, int trackingFunnelInfoID, long from, long to, int userId)
        {
            try
            {
                if (!checkAuthencation(webID, userId)) return new UnauthorizedResult();
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
                        int downcount = steps.Count;
                        var matchStep = steps[0];
                        for (int j = 0; j < statisticData.Count; j++)
                        {
                            
                            if (matchStep.typeUrl == "match")
                            {
                                if (statisticData[j].Equals(matchStep.stepUrl))
                                {
                                    downcount -= 1;
                                    if (downcount == 0)
                                    {
                                        count += 1;
                                        matchStep = steps[0];
                                        downcount = steps.Count;
                                    }else
                                    matchStep = steps[steps.IndexOf(matchStep) + 1];

                                }
                            }
                            else if (matchStep.typeUrl == "contain")
                            {
                                if (statisticData[j].Contains(matchStep.stepUrl))
                                {
                                    downcount -= 1;
                                    if (downcount == 0)
                                    {
                                        count += 1;
                                        matchStep = steps[0];
                                        downcount = steps.Count;
                                    }
                                    else
                                    matchStep = steps[steps.IndexOf(matchStep) + 1];

                                }
                            }
                            else if (matchStep.typeUrl == "start-with")
                            {
                                if (statisticData[j].LastIndexOf(matchStep.stepUrl) == 0)
                                {
                                    downcount -= 1;
                                    if (downcount == 0)
                                    {
                                        count += 1;
                                        matchStep = steps[0];
                                        downcount = steps.Count;
                                    }
                                    else
                                    matchStep = steps[steps.IndexOf(matchStep) + 1];

                                }
                            }
                            else if (matchStep.typeUrl == "end-with")
                            {
                                if ((statisticData[j].LastIndexOf(matchStep.stepUrl) + matchStep.stepUrl.Length) == statisticData[j].Length)
                                {
                                    downcount -= 1;
                                    if (downcount == 0)
                                    {
                                        count += 1;
                                        matchStep = steps[0];
                                        downcount = steps.Count;
                                    }
                                    else
                                    matchStep = steps[steps.IndexOf(matchStep) + 1];

                                }
                            }
                            else if (matchStep.typeUrl == "regex")
                            {
                                Regex reg = new Regex(matchStep.stepUrl);
                                if (reg.IsMatch(statisticData[j]))
                                {
                                    downcount -= 1;
                                    if (downcount == 0)
                                    {
                                        count += 1;
                                        matchStep = steps[0];
                                        downcount = steps.Count;
                                    }
                                    matchStep = steps[steps.IndexOf(matchStep) + 1];

                                }
                            }
                        }
                    }
                    StatisticFunnelResponse statisticFunnelResponse =
                        new StatisticFunnelResponse(trackingInfoFunnelSteps[i - 1].name, trackingInfoFunnelSteps[i - 1].stepUrl
                        , trackingInfoFunnelSteps[i - 1].typeUrl, count);
                    statisticFunnelResponses.Add(statisticFunnelResponse);
                }
                return new OkObjectResult(new
                {
                    name = trackingFunnelInfo.Name,
                    statistic = statisticFunnelResponses

                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n==========================================================================\nERROR: " + ex.Message);
                return new UnprocessableEntityResult();
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
