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


        public static void Main(string[] args)
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
                currentID = updateStatisticData();
                Thread.Sleep(6 * 60 * 1000);
            }



            int updateStatisticData()
            {
                List<TrackedHeatmapData> trackedList = context.TrackedHeatmapData.Where(s => s.TrackedHeatmapDataId > currentID)
                .Where(s => EVENT_TYPE_LIST.Contains(s.EventType)).ToList();

                //scroll data
                List<TrackedHeatmapData> trackedScrollData = context.TrackedHeatmapData.Where(s => s.EventType == 2 && s.TrackedHeatmapDataId > currentID).ToList();
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
                            context.SaveChanges();
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
                            statisticHeatmap.StatisticData = JsonConvert.SerializeObject(statisticScrollData);
                            context.SaveChanges();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("ERROR UPDATE STATISTIC SCROLL DATA ID " + statisticHeatmap.TrackedHeatmapDataId);
                        }
                    }
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
                        List<RequestData> requestData = new List<RequestData>();
                        foreach (var subItem in subTrackedList)
                        {
                            //List<RequestData> requestData = new List<RequestData>();

                            List<TempData> data = new List<TempData>();
                            try
                            {
                                data = JsonConvert.DeserializeObject<List<TempData>>(subItem.Data);
                            }
                            catch (Exception)
                            {
                            }
                            if (data != null || data.Count != 0)
                                foreach (var item in data)
                                {
                                    requestData.Add(new RequestData(subItem.TrackedHeatmapDataId, item.selector, item.width, item.height, item.offsetX, item.offsetY));
                                }

                        }

                        var client = new RestClient("http://localhost:7777/dom/coordinates/" + url);
                        Console.WriteLine("\n\n\n\n\n===================================================================================\n" + "start call api: " + client.BaseUrl.ToString());
                        Console.WriteLine("request body:" + requestData);
                        // client.Authenticator = new HttpBasicAuthenticator(username, password);
                        var request = new RestRequest();
                        request.AddParameter("url", url);
                        request.AddHeader("Content-Type", "application/json");
                        request.AddJsonBody(requestData);
                        var response = client.Post(request);
                        var content = response.Content;

                        Console.WriteLine(" done call api\n" + "\n===================================================================================\n" + "Response:\n");
                        Console.WriteLine(content.ToString());

                        List<ResponseData> responseData = new List<ResponseData>();
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Include,
                            MissingMemberHandling = MissingMemberHandling.Ignore
                        };
                        responseData = JsonConvert.DeserializeObject<List<ResponseData>>(content);

                        List<int> uniqueTrackedHeatmapDataID = responseData.Select(s => s.trackedHeatmapDataID).Distinct().ToList();

                        foreach (var id in uniqueTrackedHeatmapDataID)
                        {
                            List<ResponseData> subData = responseData.Where(s => s.trackedHeatmapDataID == id).ToList();
                            StatisticHeatmap statisticHeatmap = new StatisticHeatmap();
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
                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {
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
                    List<string> distinctData = JsonConvert.DeserializeObject<List<string>>(trackedFunnelData.TrackedSteps).Distinct().ToList();
                    StatisticFunnel statisticFunnel = context.StatisticFunnel.Where(s => s.TrackedFunnelDataId == trackedFunnelData.TrackedFunnelDataId).FirstOrDefault();
                    if (statisticFunnel != null)
                    {
                        statisticFunnel.StatisticData = JsonConvert.SerializeObject(distinctData);
                        context.SaveChanges();
                    }
                    else
                    {
                        statisticFunnel = new StatisticFunnel();
                        statisticFunnel.TrackedFunnelDataId = trackedFunnelData.TrackedFunnelDataId;
                        statisticFunnel.StatisticData = JsonConvert.SerializeObject(distinctData);
                        context.StatisticFunnel.Add(statisticFunnel);
                        context.SaveChanges();
                    }
                }

                return max;

            }

        }


    }
}
