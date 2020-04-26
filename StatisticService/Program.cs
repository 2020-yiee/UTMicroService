using Newtonsoft.Json;
using RestSharp;
using StatisticService.EFModels;
using StatisticService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace StatisticService
{
    class Program
    {
        private static readonly DBUTContext context = new DBUTContext();


        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            List<int> EVENT_TYPE_LIST = new List<int>();
            int currentClickHoverID = 4233;
            int currentScrollID = 4185;
            int currentFunnelID = 912;

            Console.WriteLine("Hello World!");
            EVENT_TYPE_LIST.Add(0);
            EVENT_TYPE_LIST.Add(1);
            while (true)
            {
                Console.WriteLine("\n\n\n\n*********\nNEW ROUND WITH ID \nCurrent click hover id =" + currentClickHoverID + "\nCurrent scroll id =" + currentScrollID +
                    "\nCurrent funnel id =" + currentFunnelID + "\n*********\n");

                currentScrollID = await updateStatisticScrollDataAsync();
                currentFunnelID = await updateStatisticFunnelDataAsync();
                currentClickHoverID = await updateStatisticClickHoverDataAsync();

                Thread.Sleep(1 * 5 * 1000);
            }

            //============================================================================================================================================================
            async System.Threading.Tasks.Task<int> updateStatisticFunnelDataAsync()
            {
                int max = -1;

                //funnel
                foreach (var trackedFunnelData in context.TrackedFunnelData.ToList())
                {

                    StatisticFunnel statisticFunnel = context.StatisticFunnel.Where(s => s.TrackedFunnelDataId == trackedFunnelData.TrackedFunnelDataId).FirstOrDefault();
                    if (statisticFunnel == null)
                    {
                        Console.WriteLine("ADD STATISTIC FUNNEL FOR ID " + trackedFunnelData.TrackedFunnelDataId);
                        StatisticFunnel newStatisticFunnel = new StatisticFunnel();
                        newStatisticFunnel.TrackedFunnelDataId = trackedFunnelData.TrackedFunnelDataId;
                        newStatisticFunnel.StatisticData = trackedFunnelData.TrackedSteps;
                        context.StatisticFunnel.Add(newStatisticFunnel);
                        await context.SaveChangesAsync();

                        max = newStatisticFunnel.TrackedFunnelDataId;
                    }
                    else
                    {
                        if (statisticFunnel.StatisticData != trackedFunnelData.TrackedSteps)
                        {
                            statisticFunnel.StatisticData = trackedFunnelData.TrackedSteps;
                            context.SaveChanges();
                            Console.WriteLine("UPDATE FUNNEL DATA FOR ID " + statisticFunnel.TrackedFunnelDataId);
                        }

                        max = statisticFunnel.TrackedFunnelDataId;
                    }
                }
                return max;
            }

            //============================================================================================================================================================
            async System.Threading.Tasks.Task<int> updateStatisticScrollDataAsync()
            {
                int max = currentScrollID;
                //scroll data
                List<TrackedHeatmapData> trackedScrollData = context.TrackedHeatmapData.Where(s => s.EventType == 2 && s.TrackedHeatmapDataId > currentScrollID).ToList();
                try
                {
                    foreach (var item in trackedScrollData)
                    {
                        ScrollDatas scrollData = JsonConvert.DeserializeObject<ScrollDatas>(item.Data);
                        List<ScrollData> scrolls = scrollData.scroll;

                        List<double> positions = scrolls.Select(s => s.position).Distinct().ToList();

                        List<ScrollData> maps = new List<ScrollData>();
                        foreach (var position in positions)
                        {
                            List<double> durations = scrolls.Where(s => s.position == position).Select(s => s.duration).ToList();
                            double sum = durations.Sum();
                            maps.Add(new ScrollData(position, sum));
                        }


                        List<double> statisticScroll = new List<double>();
                        foreach (var map in maps)
                        {

                            double top = map.duration < 10 ? map.duration : 10;
                            if (map.duration >= 1)
                                for (int i = 1; i <= top; i++)
                                {
                                    statisticScroll.Add(map.position);
                                }
                        }

                        StatisticScrollData statisticScrollData = new StatisticScrollData();
                        statisticScrollData.documentHeight = scrollData.documentHeight;
                        statisticScrollData.scroll = statisticScroll;


                        StatisticHeatmap statisticHeatmap = context.StatisticHeatmap
                            .Where(s => s.TrackedHeatmapDataId == item.TrackedHeatmapDataId).FirstOrDefault();
                        if (statisticHeatmap == null)
                        {
                            statisticHeatmap = new StatisticHeatmap();
                            statisticHeatmap.TrackedHeatmapDataId = item.TrackedHeatmapDataId;
                            statisticHeatmap.StatisticData = JsonConvert.SerializeObject(statisticScrollData);
                            try
                            {

                                context.StatisticHeatmap.Add(statisticHeatmap);
                                await context.SaveChangesAsync();
                                Console.WriteLine("STATISTIC SCROLL DATA FOR ID " + statisticHeatmap.TrackedHeatmapDataId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                Console.WriteLine("ERROR ADD STATISTIC SCROLL DATA");
                            }
                        }
                        else
                        {
                            try
                            {

                                if (JsonConvert.SerializeObject(statisticScrollData) != statisticHeatmap.StatisticData)
                                {

                                    Console.WriteLine("UPDATE STATISTIC SCROLL DATA FOR ID " + statisticHeatmap.TrackedHeatmapDataId);
                                    statisticHeatmap.StatisticData = JsonConvert.SerializeObject(statisticScrollData);
                                    await context.SaveChangesAsync();
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("ERROR UPDATE STATISTIC SCROLL DATA ID " + statisticHeatmap.TrackedHeatmapDataId);
                            }
                        }
                        max = item.TrackedHeatmapDataId;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("scrool data error: do not add documentHeight");
                }
                return max;
            }

            //============================================================================================================================================================
            
            async System.Threading.Tasks.Task<int> updateStatisticClickHoverDataAsync()
            {
                //click and hover
                List<TrackedHeatmapData> trackedList = context.TrackedHeatmapData.Where(s => s.TrackedHeatmapDataId > currentClickHoverID)
                .Where(s => EVENT_TYPE_LIST.Contains(s.EventType)).ToList();


                int max = currentClickHoverID;

                foreach (var item in trackedList)
                {
                    if (item.TrackedHeatmapDataId > max) max = item.TrackedHeatmapDataId;
                }

                List<string> uniqueUrl = trackedList.Where(s => !s.TrackingUrl.Contains("3ktruongphi")).Select(s => s.TrackingUrl).Distinct().ToList();

                foreach (var url in uniqueUrl)
                {
                    try
                    {
                        List<TrackedHeatmapData> subTrackedList = trackedList.Where(s => s.TrackingUrl == url).ToList();
                        if(subTrackedList.Count > 100)
                        {
                            int count = 0;
                            int lastCount = 0;
                            while(count < subTrackedList.Count)
                            {
                                if(subTrackedList.Count - count > 100)
                                {
                                    count += 100;
                                }
                                else
                                {
                                    count = subTrackedList.Count;
                                }
                                List<TrackedHeatmapData> list = subTrackedList.GetRange(lastCount, count-lastCount);
                                List<List<ResponseData>> responseDatas = new List<List<ResponseData>>();
                                string content = callClickAndHoverApi(list, url);
                                try
                                {
                                    responseDatas = JsonConvert.DeserializeObject<List<List<ResponseData>>>(content);
                                }
                                catch (Exception ex)
                                {

                                    Console.WriteLine("ERROR from url " + url + " :" + ex.Message);
                                    max = subTrackedList.Last().TrackedHeatmapDataId;
                                    Console.WriteLine("Error browser service. stop.");
                                    continue;
                                    //return max - 1;
                                }

                                addStatisticClickAndHoverData(responseDatas);
                                lastCount = count;
                            }
                        }
                        else
                        {
                            List<List<ResponseData>> responseDatas = new List<List<ResponseData>>();
                            string content = callClickAndHoverApi(subTrackedList, url);
                            try
                            {
                                responseDatas = JsonConvert.DeserializeObject<List<List<ResponseData>>>(content);
                            }
                            catch (Exception ex)
                            {

                                Console.WriteLine("ERROR from url " + url + " :" + ex.Message);
                                max = subTrackedList.Last().TrackedHeatmapDataId;
                                Console.WriteLine("Error browser service. stop.");
                                continue;
                                //return max - 1;
                            }

                            addStatisticClickAndHoverData(responseDatas);
                        }
                          
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR from url " + url + " :" + ex.Message);
                        continue;
                    }

                }

                Console.WriteLine("Statistic done .");
                return max;

            }

            //==========================================================================================================

            string callClickAndHoverApi(List<TrackedHeatmapData> subTrackedList,string url)
            {
                List<List<RequestData>> requestData = new List<List<RequestData>>();
                List<RequestData> mobile = new List<RequestData>();
                List<RequestData> tablet = new List<RequestData>();
                List<RequestData> desktop = new List<RequestData>();

                foreach (var subItem in subTrackedList)
                {
                    //List<RequestData> requestData = new List<RequestData>();

                    List<data> data = new List<data>();
                    try
                    {
                        data = JsonConvert.DeserializeObject<List<data>>(subItem.Data);
                    }
                    catch (Exception)
                    {
                    }
                    if (data != null || data.Count != 0)
                        foreach (var item in data)
                        {
                            if (subItem.ScreenWidth <= 540)
                            {
                                mobile.Add(new RequestData(subItem.TrackedHeatmapDataId, item.selector, item.width, item.height, item.offsetX, item.offsetY));
                            }
                            if (540 < subItem.ScreenWidth && subItem.ScreenWidth <= 768)
                            {
                                tablet.Add(new RequestData(subItem.TrackedHeatmapDataId, item.selector, item.width, item.height, item.offsetX, item.offsetY));
                            }
                            if (subItem.ScreenWidth > 768)
                            {
                                desktop.Add(new RequestData(subItem.TrackedHeatmapDataId, item.selector, item.width, item.height, item.offsetX, item.offsetY));
                            }
                        }

                }

                requestData.Add(mobile);
                requestData.Add(tablet);
                requestData.Add(desktop);

                var client = new RestClient("http://localhost:8082/dom/coordinates/" + url);
                Console.WriteLine("start call api: " + client.BaseUrl.ToString());
                // client.Authenticator = new HttpBasicAuthenticator(username, password);
                var request = new RestRequest();
                //request.AddParameter("url", url);
                request.AddHeader("Content-Type", "application/json");
                //var json = JsonConvert.SerializeObject(requestData);
                request.AddJsonBody(requestData);
                request.Timeout = 60000;
                var response = client.Post(request);
                var content = response.Content;
                Console.WriteLine(" receive response");
                int countTime = 1;
                while ((content == null || response.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest)) && countTime <= 5)
                {
                    Console.WriteLine("Error browser service. try again.");
                    countTime += 1;
                    Thread.Sleep(5000);
                    response = client.Post(request);
                    content = response.Content;
                }
                if (content == null)
                {
                    Console.WriteLine("Error browser service. tried 5 time already and stop.");
                    return null;
                }
                Console.WriteLine(" done call api");
                Console.WriteLine(response.StatusCode.ToString());
                Console.WriteLine("response error message: " + response.ErrorMessage);
                return content;
            }


            //======================================================================================================


            async void addStatisticClickAndHoverData(List<List<ResponseData>> responseDatas)
            {
                using (var context = new DBUTContext())
                {
                    List<ResponseData> responseData = new List<ResponseData>();
                    foreach (var item in responseDatas)
                    {
                        responseData.AddRange(item);
                    }

                    //responseData = JsonConvert.DeserializeObject<List<ResponseData>>(content);

                    List<int> uniqueTrackedHeatmapDataID = responseData.Select(s => s.trackedHeatmapDataID).Distinct().ToList();

                    foreach (var id in uniqueTrackedHeatmapDataID)
                    {
                        List<ResponseData> subData = responseData.Where(s => s.trackedHeatmapDataID == id).ToList();
                        StatisticHeatmap statisticHeatmap = context.StatisticHeatmap.Where(s => s.TrackedHeatmapDataId == id).FirstOrDefault();
                        List<StatisticData> statisticDatas = new List<StatisticData>();
                        foreach (var item in subData)
                        {
                            statisticDatas.Add(new StatisticData(item.x, item.y));
                        }
                        if (statisticHeatmap == null)
                        {
                            statisticHeatmap = new StatisticHeatmap();
                            statisticHeatmap.TrackedHeatmapDataId = id;
                            statisticHeatmap.StatisticData = JsonConvert.SerializeObject(statisticDatas);

                            try
                            {
                                context.StatisticHeatmap.Add(statisticHeatmap);
                                await context.SaveChangesAsync();
                                Console.WriteLine("ADD STATISTIC CLICK HOVER ID " + statisticHeatmap.TrackedHeatmapDataId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                continue;
                            }
                        }
                        else
                        {
                            try
                            {
                                if (statisticHeatmap.StatisticData != JsonConvert.SerializeObject(statisticDatas))
                                {

                                    try
                                    {
                                        await context.SaveChangesAsync();
                                        Console.WriteLine("UPDATE STATISTIC CLICK HOVER ID " + statisticHeatmap.TrackedHeatmapDataId);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        continue;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }

                    }
                }
            }

        }


    }
}
