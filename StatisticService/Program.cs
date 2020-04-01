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
            int currentID = -1;

            Console.WriteLine("Hello World!");
            EVENT_TYPE_LIST.Add(0);
            EVENT_TYPE_LIST.Add(1);
            //updateStatisticData();
            while (true)
            {
                Console.WriteLine("\n\n\n\n*********\nNEW ROUND WITH ID " + currentID + "\n*********\n");
                currentID = await updateStatisticData();
                Thread.Sleep(2 * 60 * 1000);
            }


            

            async System.Threading.Tasks.Task<int> updateStatisticData()
            {
                List<TrackedHeatmapData> trackedList = context.TrackedHeatmapData.Where(s => s.TrackedHeatmapDataId > currentID)
                .Where(s => EVENT_TYPE_LIST.Contains(s.EventType)).ToList();

                //scroll data
                List<TrackedHeatmapData> trackedScrollData = context.TrackedHeatmapData.Where(s => s.EventType == 2 && s.TrackedHeatmapDataId > currentID).ToList();
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
                                Console.WriteLine("ERROR ADD STATISTIC SCROLL DATA");
                            }
                        }
                        else
                        {
                            try
                            {

                                Console.WriteLine("STATISTIC SCROLL DATA FOR ID " + statisticHeatmap.TrackedHeatmapDataId);
                                statisticHeatmap.StatisticData = JsonConvert.SerializeObject(statisticScrollData);
                                await context.SaveChangesAsync();
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("ERROR UPDATE STATISTIC SCROLL DATA ID " + statisticHeatmap.TrackedHeatmapDataId);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("scrool data error: do not add documentHeight");
                }


                //click and hover
                int max = currentID;

                foreach (var item in trackedList)
                {
                    if (item.TrackedHeatmapDataId > max) max = item.TrackedHeatmapDataId;
                }

                List<string> uniqueUrl = trackedList.Select(s => s.TrackingUrl).Distinct().ToList();

                foreach (var url in uniqueUrl)
                {
                    try
                    {
                        List<TrackedHeatmapData> subTrackedList = trackedList.Where(s => s.TrackingUrl == url).ToList();
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
                                    if (subItem.ScreenWidth < 540)
                                    {
                                        mobile.Add(new RequestData(subItem.TrackedHeatmapDataId, item.selector, item.width, item.height, item.offsetX, item.offsetY));
                                    }
                                    if (540 <= subItem.ScreenWidth && subItem.ScreenWidth < 768)
                                    {
                                        tablet.Add(new RequestData(subItem.TrackedHeatmapDataId, item.selector, item.width, item.height, item.offsetX, item.offsetY));
                                    }
                                    if (subItem.ScreenWidth >= 768)
                                    {
                                        desktop.Add(new RequestData(subItem.TrackedHeatmapDataId, item.selector, item.width, item.height, item.offsetX, item.offsetY));
                                    }
                                }

                        }

                        requestData.Add(mobile);
                        requestData.Add(tablet);
                        requestData.Add(desktop);

                        var client = new RestClient("http://localhost:7777/dom/coordinates/" + url);
                        Console.WriteLine("\n===================================================================================\n" + "start call api: " + client.BaseUrl.ToString());
                        // client.Authenticator = new HttpBasicAuthenticator(username, password);
                        var request = new RestRequest();
                        request.AddParameter("url", url);
                        request.AddHeader("Content-Type", "application/json");
                        var json = JsonConvert.SerializeObject(requestData);
                        request.AddJsonBody(requestData);
                        var response = client.Post(request);
                        var content = response.Content;

                        Console.WriteLine(" done call api" + "\n===================================================================================\n" + "Response:\n");
                        Console.WriteLine(response.StatusCode +" "+response.ErrorMessage);
                        List<ResponseData> responseData = new List<ResponseData>();
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Include,
                            MissingMemberHandling = MissingMemberHandling.Ignore
                        };

                        List<List<ResponseData>> responseDatas = JsonConvert.DeserializeObject<List<List<ResponseData>>>(content);
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
                            if (statisticHeatmap == null)
                            {
                                statisticHeatmap = new StatisticHeatmap();
                                statisticHeatmap.TrackedHeatmapDataId = id;
                                List<StatisticData> statisticDatas = new List<StatisticData>();
                                foreach (var item in subData)
                                {
                                    statisticDatas.Add(new StatisticData(item.x, item.y));
                                }
                                statisticHeatmap.StatisticData = JsonConvert.SerializeObject(statisticDatas);
                                try
                                {
                                    context.StatisticHeatmap.Add(statisticHeatmap);
                                    await context.SaveChangesAsync();
                                }
                                catch (Exception ex)
                                {
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\n===================================================================================\n"
                            + "ERROR from url " + url + " :" + ex.Message
                            + "\n===================================================================================\n");
                        continue;
                    }

                }
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
                    }
                    else
                    {
                        statisticFunnel.StatisticData = trackedFunnelData.TrackedSteps;
                        await context.SaveChangesAsync();
                    }
                }

                Console.WriteLine("Statistic done .");
                return max;

            }

        }


    }
}
