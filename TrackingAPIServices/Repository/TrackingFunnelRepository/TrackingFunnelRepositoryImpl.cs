using HeatMapAPIServices.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StatisticAPIService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackingAPIServices.EFModels;
using TrackingAPIServices.Models;

namespace TrackingAPIServices.Repository.TrackingFunnelRepository
{
    public class TrackingFunnelRepositoryImpl : ITrackingFunnelRepository
    {

        private List<string> TYPEURL = new List<string>();

        public TrackingFunnelRepositoryImpl()
        {
            TYPEURL.Add("match");
            TYPEURL.Add("start-with");
            TYPEURL.Add("end-with");
            TYPEURL.Add("contain");
            TYPEURL.Add("regex");
        }
        //-----------------------------------------------funnel-----------------------------------------------


        // get tracking funnel info
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

        // create tracking funnel info
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

        //update step and name of tracking funnel info
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

        //delete funnel tracking info
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

        //create funnel tracked data
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

        //get statistic funnel
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

        private Boolean checkValidTrackingFunnelInfo(CreateTrackingFunnelInforRequest request, int userId)
        {
            if (!checkWebsiteAuthencation(request.webID, userId, true)) return false;
            if (checkTrackingFunnelInfoExisted(request.webID, request.steps.ToString(), request.name)) return false;
            if (!checkStepDomainUrl(request.webID, request.steps)) return false;
            return true;
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
    }
}
