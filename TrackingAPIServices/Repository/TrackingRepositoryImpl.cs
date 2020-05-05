using HeatMapAPIServices.Controllers;
using HeatMapAPIServices.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using StatisticAPIService.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackingAPIServices.EFModels;
using TrackingAPIServices.Models;

namespace HeatMapAPIServices.Repository
{
    public class TrackingRepositoryImpl : ITrackingRepository
    {

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
            using (var context = new DBUTContext())
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
        }

        public bool createTrackedHeatmapData(SaveDataRequest request)
        {
            using (var context = new DBUTContext())
            {
                try
                {
                    Website website = context.Website.Where(s => s.WebId == request.webID && s.Removed == false).FirstOrDefault();
                    if (website == null) return false;
                    Access access = context.Access.FirstOrDefault(s => s.OrganizationId == website.OrganizationId && s.Role == 1);
                    if (access == null) return false;
                    User user = context.User.FirstOrDefault(s => s.UserId == access.UserId && s.Actived == true);
                    if (user == null) return false;
                    List<TrackingHeatmapInfo> trackingHeatmapInfos = context.TrackingHeatmapInfo.Where(s =>
                    s.Tracking == true && s.WebId == request.webID).ToList();
                    Boolean has = false;
                    foreach (var trackingFunnelInfo in trackingHeatmapInfos)
                    {
                        switch (trackingFunnelInfo.TypeUrl)
                        {
                            case "match":
                                if (trackingFunnelInfo.TrackingUrl == request.trackingUrl) has = true;
                                break;
                            case "contain":
                                if (request.trackingUrl.Contains(trackingFunnelInfo.TrackingUrl)) has = true;
                                break;
                            case "start-with":
                                if (request.trackingUrl.LastIndexOf(trackingFunnelInfo.TrackingUrl) == 0) has = true;
                                break;
                            case "end-with":
                                if ((request.trackingUrl.LastIndexOf(trackingFunnelInfo.TrackingUrl) + trackingFunnelInfo.TrackingUrl.Length)
                                    == request.trackingUrl.Length) has = true;
                                break;
                            case "regex":
                                Regex reg = new Regex(trackingFunnelInfo.TrackingUrl);
                                if (reg.IsMatch(request.trackingUrl)) has = true;
                                break;
                        }
                    }
                    if (has == false) return false;

                    TrackedHeatmapData trackedData = new TrackedHeatmapData();
                    trackedData.TrackingUrl = request.trackingUrl;
                    trackedData.WebId = request.webID;
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
        }
        //----------------------------------statistic heatmap------------------------------------
        public List<StatisticHeatmap> getstatisticHeatmapData(int trackedHeatmapInfoID)
        {
            using (var context = new DBUTContext())
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
        }

        //----------------------------------tracking heatmap info--------------------------------

        private bool checkTrackingHeatmapInfoExisted(CreateTrackingHeatmapInforRequest request)
        {
            using (var context = new DBUTContext())
            {
                List<string> listTrackingUrl = context.TrackingHeatmapInfo.Where(s =>
                s.WebId == request.webID && s.Removed == false && s.TypeUrl == request.typeUrl)
                .ToList().Select(s => s.TrackingUrl).ToList();
                List<string> listName = context.TrackingHeatmapInfo.Where(
                    s => s.WebId == request.webID && s.Removed == false)
                    .ToList().Select(s => s.Name).ToList();
                if (listTrackingUrl.Contains(request.trackingUrl) == true || listName.Contains(request.name) == true)
                {
                    TrackingHeatmapInfo trackingHeatmapInfo = context.TrackingHeatmapInfo.Where(s => s.TrackingUrl == request.trackingUrl &&
                    s.TypeUrl == request.typeUrl && s.Name == request.name).FirstOrDefault();
                    if (trackingHeatmapInfo != null) return true;
                }
                return false;
            }

        }

        private bool checkTrackingHeatmapInfoName(CreateTrackingHeatmapInforRequest request)
        {
            using (var context = new DBUTContext())
            {
                List<string> listName = context.TrackingHeatmapInfo.Where(
                    s => s.WebId == request.webID && s.Removed == false)
                    .ToList().Select(s => s.Name).ToList();
                if (listName.Contains(request.name)) return true;
                return false;
            }
        }
        private bool checkDomainUrl(CreateTrackingHeatmapInforRequest request)
        {
            using (var context = new DBUTContext())
            {
                try
                {
                    if (!TYPEURL.Contains(request.typeUrl)) return false;
                    string domainUrl = context.Website.Where(s => s.WebId == request.webID).Select(s => s.DomainUrl).FirstOrDefault();
                    if (request.typeUrl == "start-with" || request.typeUrl == "match")
                    {
                        if (!request.trackingUrl.Contains(domainUrl)) return false;
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
        }
        private bool checkTrackingHeatmapInfoExisted1(int webId, string name)
        {
            using (var context = new DBUTContext())
            {
                List<string> listTrackingUrl = context.TrackingHeatmapInfo.Where(s => s.WebId == webId && s.Removed == false)
                .ToList().Select(s => s.TrackingUrl).ToList();
                List<string> listName = context.TrackingHeatmapInfo.Where(s => s.WebId == webId && s.Removed == false)
                    .ToList().Select(s => s.Name).ToList();

                if (listName.Contains(name) == true) return false;
                Website web = context.Website.Where(s => s.WebId == webId).FirstOrDefault();
                return false;
            }

        }


        public IActionResult getTrackingHeatmapInfo(int websiteId, int userId)
        {
            using (var context = new DBUTContext())
            {
                try
                {
                    if (!checkWebsiteAuthencation(websiteId, userId, false)) return new UnauthorizedResult();
                    List<string> trackingHeatmapUrls = context.TrackingHeatmapInfo.Where(
                        s => s.WebId == websiteId && s.Removed == false).Select(s => s.TrackingUrl)
                        .Distinct().ToList();
                    List<TrackingHeatmapInfo> trackingInfos = new List<TrackingHeatmapInfo>();
                    foreach (var url in trackingHeatmapUrls)
                    {
                        trackingInfos.Add(context.TrackingHeatmapInfo.Last(
                            s => s.TrackingUrl == url && s.Removed == false));
                    }

                    List<TrackingHeatmapInfoResponse> trackingHeatmapInfoResponses = new List<TrackingHeatmapInfoResponse>();

                    foreach (var trackingInfo in trackingInfos)
                    {
                        List<TrackedHeatmapData> datas = new List<TrackedHeatmapData>();
                        foreach (var item in EVENT_TYPE_LIST)
                        {
                            datas.AddRange(getTrackedHeatmapDataByType(trackingInfo.TypeUrl, websiteId, trackingInfo, 1111111111, 1911111111, item));
                        }
                        if (trackingInfo.Tracking == false)
                        {
                            datas = datas.Where(s => s.CreatedAt <= trackingInfo.EndAt).ToList();
                        }
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
                        int visit = 0;
                        foreach (DateTime time in times)
                        {
                            visit += countVisit(time, datas);
                        }
                        trackingHeatmapInfoResponses.Add(new TrackingHeatmapInfoResponse(
                            trackingInfo.TrackingHeatmapInfoId,
                            trackingInfo.TrackingUrl, trackingInfo.Name, trackingInfo.CreatedAt,
                            context.User.Where(s => s.UserId == trackingInfo.AuthorId).FirstOrDefault().FullName, visit,
                            trackingInfo.Version, trackingInfo.Tracking, trackingInfo.EndAt));
                    }

                    return new OkObjectResult(trackingHeatmapInfoResponses);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    return new BadRequestResult();
                }
            }
        }

        public IActionResult getAllVersionTrackingHeatmapInfo(int trackingHeatmapInfoID, int userId)
        {
            using (var context = new DBUTContext())
            {
                TrackingHeatmapInfo trackingHeatmapInfo = context.TrackingHeatmapInfo.Where(
                    s => s.TrackingHeatmapInfoId == trackingHeatmapInfoID && s.Removed == false).FirstOrDefault();
                if (trackingHeatmapInfo == null) return new NotFoundResult();
                if (!checkWebsiteAuthencation(trackingHeatmapInfo.WebId, userId, false)) return new UnauthorizedResult();
                List<TrackingHeatmapInfo> trackingHeatmapInfos = context.TrackingHeatmapInfo.Where(
                    s => s.TrackingUrl == trackingHeatmapInfo.TrackingUrl
                    && s.Removed == false).ToList();
                List<TrackingHeatmapInfoResponse> responses = new List<TrackingHeatmapInfoResponse>();
                foreach (var trackingInfo in trackingHeatmapInfos)
                {
                    List<TrackedHeatmapData> datas = context.TrackedHeatmapData.Where(s => s.WebId == trackingInfo.WebId)
                            .Where(s => s.TrackingUrl == trackingInfo.TrackingUrl
                            && s.CreatedAt >= trackingInfo.CreatedAt)
                            .ToList();
                    if (trackingInfo.Tracking == false)
                    {
                        datas = datas.Where(s => s.CreatedAt <= trackingInfo.EndAt).ToList();
                    }
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
                    int visit = 0;
                    foreach (DateTime time in times)
                    {
                        visit += countVisit(time, datas);
                    }
                    responses.Add(new TrackingHeatmapInfoResponse(
                            trackingInfo.TrackingHeatmapInfoId,
                            trackingInfo.TrackingUrl, trackingInfo.Name, trackingInfo.CreatedAt,
                            context.User.Where(s => s.UserId == trackingInfo.AuthorId).FirstOrDefault().FullName, visit,
                            trackingInfo.Version, trackingInfo.Tracking, trackingInfo.EndAt));
                }

                return new OkObjectResult(responses);
            }
        }

        private Boolean checkWebsiteAuthencation(int websiteId, int userId, bool edit)
        {
            using (var context = new DBUTContext())
            {
                if (context.User.FirstOrDefault(s => s.UserId == userId) == null) return false;
                List<int> orgIds = context.Access.Where(s => s.UserId == userId).Select(s => s.OrganizationId).ToList();
                if (orgIds == null || orgIds.Count == 0) return false;
                Website website = context.Website.FirstOrDefault(s => s.WebId == websiteId);
                if (website == null) return false;
                if (!orgIds.Contains(website.OrganizationId)) return false;
                if (edit)
                {
                    if (context.Access.Where(s => s.OrganizationId == website.OrganizationId &&
                 s.UserId == userId).Select(s => s.Role).FirstOrDefault() == 3) return false;
                }
                return true;
            }
        }

        private String takeCapturePicture(TrackingHeatmapInfo info, string captureUrl)
        {
            var client = new RestClient("https://browser-service.herokuapp.com/capture/" + info.WebId + "/" + info.TrackingHeatmapInfoId + "/" + captureUrl);
            // client.Authenticator = new HttpBasicAuthenticator(username, password);
            var requests = new RestRequest();
            requests.AddHeader("Content-Type", "application/json");
            requests.Timeout = 60000;
            string content = null;
            try
            {
                int count = 0;
                while (content == null && count < 4)
                {
                    var response = client.Post(requests);
                    content = response.Content;
                    count++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
            return content;
        }

        private bool checkValidTrackingHeatmapInfo(CreateTrackingHeatmapInforRequest request, int userId)
        {
            if (!checkWebsiteAuthencation(request.webID, userId, true)) return false;
            if (checkTrackingHeatmapInfoExisted(request)) return false;
            if (!checkDomainUrl(request)) return false;
            if (!TYPEURL.Contains(request.typeUrl)) return false;
            if (checkTrackingHeatmapInfoName(request)) return false;
            return true;
        }



        public IActionResult createHeatmapTrackingInfo(CreateTrackingHeatmapInforRequest request, int userId)
        {
            using (var context = new DBUTContext())
            {
                TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                if (!checkValidTrackingHeatmapInfo(request, userId))
                {
                    return new BadRequestResult();
                }
                TrackingHeatmapInfo info = new TrackingHeatmapInfo();
                info.WebId = request.webID;
                info.Name = request.name;
                info.TrackingUrl = request.trackingUrl;
                info.Removed = false;
                info.CreatedAt = (long)timeSpan.TotalSeconds;
                info.TypeUrl = request.typeUrl;
                info.AuthorId = userId;
                info.Version = "Version 1";
                info.Tracking = true;
                info.EndAt = 0;
                try
                {
                    context.TrackingHeatmapInfo.Add(info);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error " + e.Message);
                    return new UnprocessableEntityResult();
                }

                string content = takeCapturePicture(info, request.captureUrl);
                if (content == null)
                {
                    context.TrackingHeatmapInfo.Remove(info);
                    context.SaveChanges();
                    return new UnprocessableEntityResult();
                }
                else
                {
                    try
                    {
                        CaptureResponse captureResponse = JsonConvert.DeserializeObject<CaptureResponse>(content);
                        info.SmImageUrl = captureResponse.smImageUrl;
                        info.MdImageUrl = captureResponse.mdImageUrl;
                        info.LgImageUrl = captureResponse.lgImageUrl;
                        context.SaveChanges();
                        return new OkObjectResult(new TrackingHeatmapInfoResponse(info.TrackingHeatmapInfoId, info.TrackingUrl, info.Name,
                            info.CreatedAt, context.User.Where(s => s.UserId == info.AuthorId).FirstOrDefault().FullName, 0, info.Version,
                            info.Tracking, info.EndAt));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: " + ex.Message);
                        return new BadRequestResult();
                    }
                }

            }
        }

        public IActionResult createVersionHeatmapTrackingInfo(CreateVersionTrackingHeatmapInforRequest request, int userID)
        {
            using (var context = new DBUTContext())
            {
                TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));

                TrackingHeatmapInfo trackingHeatmapInfo = context.TrackingHeatmapInfo.LastOrDefault(s => 
                s.TrackingHeatmapInfoId == request.TrackingHeatmapInfoId && s.Removed == false);
                if (trackingHeatmapInfo == null) return new NotFoundResult();
                Website website = context.Website.FirstOrDefault(s => s.WebId == trackingHeatmapInfo.WebId);
                Access access = context.Access.FirstOrDefault(s => s.OrganizationId == website.OrganizationId && s.UserId == userID);
                if (access == null) return new UnauthorizedResult();
                if (access.Role == 3) return new UnauthorizedResult();

                TrackingHeatmapInfo lastTrackingHeatmapInfo = context.TrackingHeatmapInfo.LastOrDefault(s =>
                s.TrackingUrl == trackingHeatmapInfo.TrackingUrl && s.Name == trackingHeatmapInfo.Name && s.TypeUrl == trackingHeatmapInfo.TypeUrl);
                lastTrackingHeatmapInfo.Tracking = false;
                lastTrackingHeatmapInfo.EndAt = (long)timeSpan.TotalSeconds;

                TrackingHeatmapInfo info = new TrackingHeatmapInfo();
                info.WebId = trackingHeatmapInfo.WebId;
                info.Name = trackingHeatmapInfo.Name;
                info.TrackingUrl = trackingHeatmapInfo.TrackingUrl;
                info.Removed = false;
                info.CreatedAt = (long)timeSpan.TotalSeconds;
                info.TypeUrl = trackingHeatmapInfo.TypeUrl;
                info.AuthorId = userID;
                string[] b = trackingHeatmapInfo.Version.Split(" ");
                int c = Int32.Parse(b[1]) + 1;
                info.Version = "Version "+c;
                info.Tracking = true;
                info.EndAt = 0;
                try
                {
                    context.TrackingHeatmapInfo.Add(info);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error " + e.Message);
                    return new UnprocessableEntityResult();
                }

                string content = takeCapturePicture(info, request.captureUrl);
                if (content == null)
                {
                    context.TrackingHeatmapInfo.Remove(info);
                    context.SaveChanges();
                    return new UnprocessableEntityResult();
                }
                else
                {
                    try
                    {
                        CaptureResponse captureResponse = JsonConvert.DeserializeObject<CaptureResponse>(content);
                        info.SmImageUrl = captureResponse.smImageUrl;
                        info.MdImageUrl = captureResponse.mdImageUrl;
                        info.LgImageUrl = captureResponse.lgImageUrl;
                        context.SaveChanges();
                        return new OkObjectResult(new TrackingHeatmapInfoResponse(info.TrackingHeatmapInfoId,info.TrackingUrl,info.Name,
                            info.CreatedAt, context.User.Where(s => s.UserId == info.AuthorId).FirstOrDefault().FullName,0,info.Version,
                            info.Tracking,info.EndAt));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: " + ex.Message);
                        return new BadRequestResult();
                    }
                }
            }
        }

        public IActionResult updateTrackingHeatmapInfo(UpdateTrackingHeatmapInforRequest request, int userId)
        {
            using (var context = new DBUTContext())
            {
                TrackingHeatmapInfo trackingHeatmapInfo = context.TrackingHeatmapInfo.Where(s =>
            s.TrackingHeatmapInfoId == request.trackingHeatmapInfoID && s.Removed == false).FirstOrDefault();
                if (!checkTrackingHeatmapInfoExisted1(trackingHeatmapInfo.WebId, request.newName)) return new BadRequestResult();
                if (!checkWebsiteAuthencation(trackingHeatmapInfo.WebId, userId, true)) return new BadRequestResult();
                try
                {
                    TrackingHeatmapInfo info = context.TrackingHeatmapInfo
                        .Where(s => s.TrackingHeatmapInfoId == request.trackingHeatmapInfoID
                        && s.Removed == false).FirstOrDefault();
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
        }
        public bool deleteTrackingHeatmapInfo(int trackingHeatmapInfoId, int userId, bool isVersion)
        {
            using (var context = new DBUTContext())
            {
                TrackingHeatmapInfo trackingHeatmapInfo = context.TrackingHeatmapInfo.Where(s =>
                s.TrackingHeatmapInfoId == trackingHeatmapInfoId
                && s.Removed == false).FirstOrDefault();
                if (trackingHeatmapInfo == null) return false;
                Website website = context.Website.FirstOrDefault(s => s.WebId == trackingHeatmapInfo.WebId);
                if (context.Access.FirstOrDefault(s => s.OrganizationId == website.OrganizationId
                 && s.UserId == userId).Role != 1)
                {
                    if (trackingHeatmapInfo.AuthorId != userId) return false;
                }
                if (!checkWebsiteAuthencation(trackingHeatmapInfo.WebId, userId, true)) return false;
                try
                {
                    if (isVersion)
                    {
                        TrackingHeatmapInfo info = context.TrackingHeatmapInfo
                            .Where(s => s.TrackingHeatmapInfoId == trackingHeatmapInfoId)
                            .Where(s => s.Removed == false)
                            .FirstOrDefault();
                        if (info == null) return false;
                        info.Removed = true;
                        TrackingHeatmapInfo trackingHeatmapPre = context.TrackingHeatmapInfo.FirstOrDefault(
                            s => s.EndAt == info.CreatedAt);
                        if(trackingHeatmapPre != null)
                        {
                            trackingHeatmapPre.Tracking = true;
                            trackingHeatmapPre.EndAt = 0;
                        } 
                    }
                    else
                    {
                        List<TrackingHeatmapInfo> datas = context.TrackingHeatmapInfo
                            .Where(s => s.TrackingUrl == trackingHeatmapInfo.TrackingUrl)
                            .Where(s => s.Removed == false)
                            .ToList();
                        if (datas == null || datas.Count == 0) return false;
                        foreach (var data in datas)
                        {
                            data.Removed = true;
                        }
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



        //----------------------------tracking funnel info-----------------------------------

        private bool checkTrackingFunnelInfoExisted(int webId, string steps, string name)
        {
            using (var context = new DBUTContext())
            {
                List<string> listSteps = context.TrackingFunnelInfo.Where(s => s.WebId == webId && s.Removed == false)
                .ToList().Select(s => s.Steps).ToList();
                List<string> listName = context.TrackingFunnelInfo.Where(s => s.WebId == webId && s.Removed == false)
                    .ToList().Select(s => s.Name).ToList();

                if (listSteps.Contains(steps) == true || listName.Contains(name) == true) return true;

                return false;
            }
        }

        private bool checkStepDomainUrl(int webId, List<Step> steps)
        {
            using (var context = new DBUTContext())
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
        }
        public IEnumerable<TrackingFunnelInfoResponse> getTrackingFunnelInfo(int websiteId, int userId)
        {
            using (var context = new DBUTContext())
            {
                try
                {
                    if (!checkWebsiteAuthencation(websiteId, userId, false)) return null;
                    IEnumerable<TrackingFunnelInfo> trackingInfos = context.TrackingFunnelInfo
                        .Where(s => s.WebId == websiteId)
                        .Where(s => s.Removed == false)
                        .ToList();

                    List<TrackingFunnelInfoResponse> trackingFunelInfoResponses = new List<TrackingFunnelInfoResponse>();

                    foreach (var trackingInfo in trackingInfos)
                    {
                        List<Step> trackingInfoFunnelSteps = JsonConvert.DeserializeObject<List<Step>>(trackingInfo.Steps);
                        List<int> trackedFunnelDataIDs = context.TrackedFunnelData
                            .Where(s => s.WebId == websiteId)
                            .Select(s => s.TrackedFunnelDataId)
                            .ToList();
                        List<StatisticFunnel> statisticFunnels = context.StatisticFunnel.Where(s => trackedFunnelDataIDs.Contains(s.TrackedFunnelDataId)).ToList();

                        var statistic = getStatisticResponse(trackingInfoFunnelSteps, statisticFunnels);
                        double conversionRate;
                        try
                        {
                            conversionRate = ((Double)statistic.Last().sessions / (Double)statistic.First().sessions) * 100;

                        }
                        catch (Exception)
                        {
                            conversionRate = 0;
                        }
                        if (Double.IsNaN(conversionRate)) conversionRate = 0;
                        trackingFunelInfoResponses.Add(new TrackingFunnelInfoResponse(trackingInfo.TrackingFunnelInfoId, trackingInfo.WebId,
                            trackingInfo.Name, trackingInfo.Steps, trackingInfo.Removed, trackingInfo.CreatedAt, trackingInfo.AuthorId,
                            context.User.Where(s => s.UserId == trackingInfo.AuthorId).FirstOrDefault().FullName,
                            conversionRate));
                    }


                    return trackingFunelInfoResponses;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    return null;
                }
            }
        }

        private Boolean checkValidTrackingFunnelInfo(CreateTrackingFunnelInforRequest request, int userId)
        {
            if (!checkWebsiteAuthencation(request.webID, userId, true)) return false;
            if (checkTrackingFunnelInfoExisted(request.webID, request.steps.ToString(), request.name)) return false;
            if (!checkStepDomainUrl(request.webID, request.steps)) return false;
            return true;
        }

        public IActionResult createFunnelTrackingInfo(CreateTrackingFunnelInforRequest request, int userId)
        {
            using (var context = new DBUTContext())
            {
                if (!checkValidTrackingFunnelInfo(request, userId)) return new BadRequestResult();
                TrackingFunnelInfo info = new TrackingFunnelInfo();
                info.WebId = request.webID;
                info.Name = request.name;
                info.Steps = JsonConvert.SerializeObject(request.steps);
                info.Removed = false;
                var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                info.CreatedAt = (long)timeSpan.TotalSeconds;
                info.AuthorId = userId;
                try
                {
                    context.TrackingFunnelInfo.Add(info);
                    context.SaveChanges();

                    List<Step> trackingInfoFunnelSteps = JsonConvert.DeserializeObject<List<Step>>(info.Steps);
                    List<int> trackedFunnelDataIDs = context.TrackedFunnelData
                        .Where(s => s.WebId == request.webID)
                        .Select(s => s.TrackedFunnelDataId)
                        .ToList();
                    List<StatisticFunnel> statisticFunnels = context.StatisticFunnel.Where(s => trackedFunnelDataIDs.Contains(s.TrackedFunnelDataId)).ToList();

                    var statistic = getStatisticResponse(trackingInfoFunnelSteps, statisticFunnels);
                    double conversionRate;
                    try
                    {
                        conversionRate = ((Double)statistic.Last().sessions / (Double)statistic.First().sessions) * 100;

                    }
                    catch (Exception)
                    {
                        conversionRate = 0;
                    }
                    if (Double.IsNaN(conversionRate)) conversionRate = 0;
                    return new OkObjectResult(new TrackingFunnelInfoResponse(info.TrackingFunnelInfoId, info.WebId, info.Name,
                        info.Steps, info.Removed, info.CreatedAt, info.AuthorId,
                        context.User.Where(s => s.UserId == info.AuthorId).FirstOrDefault().FullName, conversionRate));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    return new BadRequestResult();
                }
            }
        }

        public IActionResult updateFunnelTrackingInfo(udpateTrackingStepInfoRequest request, int userId)
        {
            using (var context = new DBUTContext())
            {
                try
                {
                    TrackingFunnelInfo trackingFunnelInfo = context.TrackingFunnelInfo
                        .Where(s => s.TrackingFunnelInfoId == request.trackingFunnelInfoID && s.Removed == false)
                        .FirstOrDefault();
                    if (trackingFunnelInfo == null) return new BadRequestResult();

                    if (!checkWebsiteAuthencation(trackingFunnelInfo.WebId, userId, true)) return new BadRequestResult();
                    if (!checkStepDomainUrl(trackingFunnelInfo.WebId, request.steps)) return new BadRequestResult();

                    //update name tracking funnel info
                    List<string> names = context.TrackingFunnelInfo.Where(s => s.WebId == trackingFunnelInfo.WebId
                    && s.Removed == false && s.TrackingFunnelInfoId != request.trackingFunnelInfoID)
                        .Select(s => s.Name).ToList();
                    if (names.Contains(request.newName)) return new BadRequestResult();

                    trackingFunnelInfo.Name = request.newName;

                    trackingFunnelInfo.Steps = JsonConvert.SerializeObject(request.steps);
                    context.SaveChanges();

                    List<Step> trackingInfoFunnelSteps = JsonConvert.DeserializeObject<List<Step>>(trackingFunnelInfo.Steps);
                    List<int> trackedFunnelDataIDs = context.TrackedFunnelData
                        .Where(s => s.WebId == trackingFunnelInfo.WebId)
                        .Select(s => s.TrackedFunnelDataId)
                        .ToList();
                    List<StatisticFunnel> statisticFunnels = context.StatisticFunnel.Where(s => trackedFunnelDataIDs.Contains(s.TrackedFunnelDataId)).ToList();

                    var statistic = getStatisticResponse(trackingInfoFunnelSteps, statisticFunnels);
                    double conversionRate = statistic.First().sessions / statistic.Last().sessions;

                    return new OkObjectResult(new TrackingFunnelInfoResponse(
                        trackingFunnelInfo.TrackingFunnelInfoId, trackingFunnelInfo.WebId, trackingFunnelInfo.Name,
                        trackingFunnelInfo.Steps, trackingFunnelInfo.Removed, trackingFunnelInfo.CreatedAt, trackingFunnelInfo.AuthorId,
                        context.User.Where(s => s.UserId == trackingFunnelInfo.AuthorId).FirstOrDefault().FullName, conversionRate));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR " + ex.Message);
                    return new UnprocessableEntityResult();
                }
            }
        }

        public bool deleteTrackingFunnelInfo(int trackingId, int userId)
        {
            using (var context = new DBUTContext())
            {
                TrackingFunnelInfo trackingFunnelInfo = context.TrackingFunnelInfo.Where(s => s.TrackingFunnelInfoId == trackingId
            && s.Removed == false).FirstOrDefault();
                if (trackingFunnelInfo == null) return false;
                Website website = context.Website.FirstOrDefault(s => s.WebId == trackingFunnelInfo.WebId);
                if (context.Access.FirstOrDefault(s => s.OrganizationId == website.OrganizationId
                 && s.UserId == userId).Role != 1)
                {
                    if (trackingFunnelInfo.AuthorId != userId) return false;
                }
                if (!checkWebsiteAuthencation(trackingFunnelInfo.WebId, userId, true)) return false;
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
        }


        //===========================tracked funnel data ==============================

        public bool createTrackedFunnelData(SaveFunnelDataRequest request)
        {
            using (var context = new DBUTContext())
            {
                try
                {
                    Website website1 = context.Website.Where(s => s.WebId == request.webID).FirstOrDefault();
                    if (website1 == null || website1.Removed == true) return false;
                    Organization organization = context.Organization.FirstOrDefault(s => s.OrganizationId == website1.OrganizationId);
                    User user = context.User.FirstOrDefault(s => s.UserId ==
                    context.Access.FirstOrDefault(a => a.OrganizationId == organization.OrganizationId
                     && a.Role == 1).UserId && s.Actived == true);
                    if (user == null) return false;
                    TrackedFunnelData datac = context.TrackedFunnelData.Where(s => s.SessionId == request.sessionID).FirstOrDefault();
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
        }

        public List<TrackedFunnelData> getTrackedFunnelData(int webID)
        {
            using (var context = new DBUTContext())
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
        }

        //=======================statistic ========================================

        public List<StatisticFunnel> getstatisticFunnelData(int trackedFunnelInfoID)
        {
            using (var context = new DBUTContext())
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
        private List<TrackedHeatmapData> getTrackedHeatmapDataByType(string typeUrl, int webID, TrackingHeatmapInfo trackingHeatmapInfo
            , int from, int to, int eventType)
        {
            using (var context = new DBUTContext())
            {

                List<TrackedHeatmapData> trackedHeatmapDatas = new List<TrackedHeatmapData>();
                switch (typeUrl)
                {
                    case "match":
                        trackedHeatmapDatas = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                           .Where(s => s.TrackingUrl == trackingHeatmapInfo.TrackingUrl)
                           .Where(s => s.CreatedAt >= trackingHeatmapInfo.CreatedAt)
                           .Where(s => s.CreatedAt >= from)
                           .Where(s => s.CreatedAt <= to)
                           .Where(s => s.EventType == eventType).ToList(); break;
                    case "contain":
                        trackedHeatmapDatas = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                            .Where(s => s.TrackingUrl.Contains(trackingHeatmapInfo.TrackingUrl))
                            .Where(s => s.CreatedAt >= trackingHeatmapInfo.CreatedAt)
                            .Where(s => s.CreatedAt >= from)
                            .Where(s => s.CreatedAt <= to)
                            .Where(s => s.EventType == eventType).ToList(); break;
                    case "start-with":
                        trackedHeatmapDatas = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                            .Where(s => s.TrackingUrl.LastIndexOf(trackingHeatmapInfo.TrackingUrl) == 0)
                            .Where(s => s.CreatedAt >= trackingHeatmapInfo.CreatedAt)
                            .Where(s => s.CreatedAt >= from)
                            .Where(s => s.CreatedAt <= to)
                            .Where(s => s.EventType == eventType).ToList(); break;
                    case "end-with":
                        trackedHeatmapDatas = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                            .Where(s => (s.TrackingUrl.LastIndexOf(trackingHeatmapInfo.TrackingUrl) + trackingHeatmapInfo.TrackingUrl.Length)
                            == s.TrackingUrl.Length)
                            .Where(s => s.CreatedAt >= trackingHeatmapInfo.CreatedAt)
                            .Where(s => s.CreatedAt >= from)
                            .Where(s => s.CreatedAt <= to)
                            .Where(s => s.EventType == eventType).ToList(); break;
                    case "regex":
                        List<TrackedHeatmapData> trackedHeatmapData = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                            .Where(s => s.CreatedAt >= trackingHeatmapInfo.CreatedAt)
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
                return trackedHeatmapDatas;
            }
        }
        public IActionResult getStatisticHeatMap(int webID, int trackingInfoID, int from, int to, int device, int userId)
        {
            using (var context = new DBUTContext())
            {
                GetStatisicHeatMap response = new GetStatisicHeatMap();
                try
                {
                    if (!checkWebsiteAuthencation(webID, userId, false)) return new UnauthorizedResult();

                    //click hover
                    TrackingHeatmapInfo trackingHeatmapInfo = context.TrackingHeatmapInfo.Where(s => s.WebId == webID
                    && s.Removed == false)
                    .Where(s => s.TrackingHeatmapInfoId == trackingInfoID)
                    .FirstOrDefault();
                    if (trackingHeatmapInfo == null) return new NotFoundResult();
                    response.name = trackingHeatmapInfo.Name;
                    response.typeUrl = trackingHeatmapInfo.TypeUrl;
                    response.trackingUrl = trackingHeatmapInfo.TrackingUrl;
                    response.version = trackingHeatmapInfo.Version;
                    if (trackingHeatmapInfo.Tracking == false)
                    {
                        to = (int)trackingHeatmapInfo.EndAt;
                    }
                    switch (device)
                    {
                        case 0: response.imageUrl = trackingHeatmapInfo.SmImageUrl; break;
                        case 1: response.imageUrl = trackingHeatmapInfo.MdImageUrl; break;
                        case 2: response.imageUrl = trackingHeatmapInfo.LgImageUrl; break;
                    }
                    foreach (var eventType in EVENT_TYPE_LIST)
                    {
                        List<TrackedHeatmapData> trackedHeatmapDatas = getTrackedHeatmapDataByType(response.typeUrl, webID,
                            trackingHeatmapInfo, from, to, eventType);
                        //if (trackedHeatmapDatas == null || trackedHeatmapDatas.Count == 0) return new NotFoundResult();
                        List<StatisticHeatmap> statisticHeatmaps = new List<StatisticHeatmap>();
                        switch (device)
                        {
                            case 0:
                                List<int> trackedHeatmapDataIds = trackedHeatmapDatas
                                    .Where(s => s.ScreenWidth <= 540)
                                    .Select(s => s.TrackedHeatmapDataId).ToList();
                                statisticHeatmaps = context.StatisticHeatmap
                                    .Where(s => trackedHeatmapDataIds.Contains(s.TrackedHeatmapDataId) == true)
                                .ToList(); break;
                            case 1:
                                trackedHeatmapDataIds = trackedHeatmapDatas
                               .Where(s => s.ScreenWidth <= 768 && s.ScreenWidth > 540)
                               .Select(s => s.TrackedHeatmapDataId).ToList();
                                statisticHeatmaps = context.StatisticHeatmap
                                    .Where(s => trackedHeatmapDataIds.Contains(s.TrackedHeatmapDataId) == true)
                                .ToList();
                                break;
                            case 2:
                                trackedHeatmapDataIds = trackedHeatmapDatas
                                    .Where(s => s.ScreenWidth > 768)
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
                                trackedDataIDs = getTrackedHeatmapDataByType(response.typeUrl, webID, trackingHeatmapInfo, from, to, 2)
                                .Where(s => s.ScreenHeight == item && s.EventType == 2 && s.ScreenWidth <= 540)
                                .Select(s => s.TrackedHeatmapDataId).ToList();
                                break;
                            case 1:
                                trackedDataIDs = getTrackedHeatmapDataByType(response.typeUrl, webID, trackingHeatmapInfo, from, to, 2)
                                    .Where(s => s.ScreenHeight == item && s.EventType == 2 && s.ScreenWidth <= 768 && s.ScreenWidth > 540)
                                    .Select(s => s.TrackedHeatmapDataId).ToList();
                                break;
                            case 2:
                                trackedDataIDs = getTrackedHeatmapDataByType(response.typeUrl, webID, trackingHeatmapInfo, from, to, 2)
                                .Where(s => s.ScreenHeight == item && s.EventType == 2 && s.ScreenWidth > 768)
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
                    List<TrackedHeatmapData> datas = new List<TrackedHeatmapData>();
                    foreach (var item in EVENT_TYPE_LIST)
                    {
                        datas.AddRange(getTrackedHeatmapDataByType(response.typeUrl, webID, trackingHeatmapInfo, from, to, item));
                    }

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
                        visit.x = ((DateTimeOffset)time).ToUnixTimeSeconds();


                        visit.y = countVisit(time, datas);

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
        }

        private int countVisit(DateTime time, List<TrackedHeatmapData> datas)
        {
            var timeList = new List<TrackedHeatmapData>();
            int currentDay = time.Day;
            int currentMonth = time.Month;
            int currentYear = time.Year;
            List<String> sessions = datas.Select(s => s.SessionId).Distinct().ToList();
            foreach (var session in sessions)
            {
                var item = datas.Where(s => s.SessionId == session).FirstOrDefault();
                DateTime t = convertTimeStamp(item.CreatedAt);
                int day = t.Day;
                int month = t.Month;
                int year = t.Year;

                if (day == currentDay && currentMonth == month && currentYear == year)
                {
                    timeList.Add(item);
                }
            }
            return timeList.Select(item => item.SessionId).Distinct().ToList().Count;
        }

        private DateTime convertTimeStamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public IActionResult getStatisticFunnel(int webID, int trackingFunnelInfoID, long from, long to, int userId)
        {
            using (var context = new DBUTContext())
            {
                try
                {
                    if (!checkWebsiteAuthencation(webID, userId, false)) return new UnauthorizedResult();
                    TrackingFunnelInfo trackingFunnelInfo = context.TrackingFunnelInfo.FirstOrDefault(s =>
                    s.TrackingFunnelInfoId == trackingFunnelInfoID && s.Removed == false);
                    List<Step> trackingInfoFunnelSteps = JsonConvert.DeserializeObject<List<Step>>(trackingFunnelInfo.Steps);
                    List<int> trackedFunnelDataIDs = context.TrackedFunnelData
                        .Where(s => s.WebId == webID && from <= s.CreatedAt && s.CreatedAt <= to)
                        .Select(s => s.TrackedFunnelDataId)
                        .ToList();
                    List<StatisticFunnel> statisticFunnels = context.StatisticFunnel.Where(s => trackedFunnelDataIDs.Contains(s.TrackedFunnelDataId)).ToList();

                    return new OkObjectResult(new
                    {
                        name = trackingFunnelInfo.Name,
                        statistic = getStatisticResponse(trackingInfoFunnelSteps, statisticFunnels)

                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n==========================================================================\nERROR: " + ex.Message);
                    return new UnprocessableEntityResult();
                }
            }
        }

        private bool Satisfying(Step f, string t)
        {
            if (f.typeUrl == "match")
            {
                if (t.Equals(f.stepUrl))
                {
                    return true;
                }
            }
            else if (f.typeUrl == "contain")
            {
                if (t.Contains(f.stepUrl))
                {
                    return true;
                }
            }
            else if (f.typeUrl == "start-with")
            {
                if (t.LastIndexOf(f.stepUrl) == 0)
                {
                    return true;
                }
            }
            else if (f.typeUrl == "end-with")
            {
                if ((t.LastIndexOf(f.stepUrl)
                    + f.stepUrl.Length) == t.Length)
                {
                    return true;
                }
            }
            else if (f.typeUrl == "regex")
            {
                Regex reg = new Regex(f.stepUrl);
                if (reg.IsMatch(t))
                {
                    return true;
                }
            }
            return false;
        }

        private int furthest(List<Step> steps, List<string> trackedSteps)
        {
            var lastIndex = 0;
            var result = 0;
            foreach (var step in steps)
            {
                var hasResult = false;
                for (int i = lastIndex; i < trackedSteps.Count; i++)
                {
                    if (Satisfying(step, trackedSteps[i]))
                    {
                        lastIndex = i + 1;
                        result += 1;
                        hasResult = true;
                        break;
                    }
                }
                if (!hasResult) return result;
            }
            return result;
        }

        private List<ResponseData> getStatisticResponse(List<Step> trackingInfoFunnelSteps, List<StatisticFunnel> statisticFunnels)
        {
            List<ResponseData> statisticFunnelResponses = new List<ResponseData>();
            for (int i = 1; i <= trackingInfoFunnelSteps.Count; i++)
            {
                List<Step> steps = trackingInfoFunnelSteps.GetRange(0, i);
                List<string> stepsUrl = steps.Select(s => s.stepUrl).ToList();
                int count = 0;
                foreach (var item in statisticFunnels)
                {
                    List<string> trackedSteps = JsonConvert.DeserializeObject<List<string>>(item.StatisticData);
                    if (furthest(steps, trackedSteps) >= steps.Count)
                    {
                        count += 1;
                    }
                }
                ResponseData statisticFunnelResponse =
                    new ResponseData(trackingInfoFunnelSteps[i - 1].name, trackingInfoFunnelSteps[i - 1].stepUrl
                    , trackingInfoFunnelSteps[i - 1].typeUrl, count);
                statisticFunnelResponses.Add(statisticFunnelResponse);
            }
            return statisticFunnelResponses;
        }


    }

}
