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
        static void Main(string[] args)
        {
            List<int> EVENT_TYPE_LIST = new List<int>();
            int currentID = -1;

            Console.WriteLine("Hello World!");
            EVENT_TYPE_LIST.Add(0);
            EVENT_TYPE_LIST.Add(1);
            //updateStatisticData();
            while (true)
            {
                Console.WriteLine(currentID);
                currentID = updateStatisticData();
                Thread.Sleep(10000);
            }



            int updateStatisticData()
            {
                
                List<TrackedHeatmapData> trackedList = context.TrackedHeatmapData.Where(s => s.TrackedHeatmapDataId > currentID).Where(s => EVENT_TYPE_LIST.Contains(s.EventType)).ToList();

                int max = currentID;
                foreach (var item in trackedList)
                {
                    if (item.TrackedHeatmapDataId > max) max = item.TrackedHeatmapDataId;
                }
                
                List<string> uniqueUrl = trackedList.Select(s => s.TrackingUrl).Distinct().ToList();

                foreach (var url in uniqueUrl)
                {
                    List<TrackedHeatmapData> subTrackedList = trackedList.Where(s => s.TrackingUrl == url).ToList();
                    List<RequestData> requestData = new List<RequestData>();
                    foreach (var subItem in subTrackedList)
                    {
                        List<TempData> data = JsonConvert.DeserializeObject<List<TempData>>(subItem.Data);
                        foreach (var item in data)
                        {
                            requestData.Add(new RequestData(subItem.TrackedHeatmapDataId, item.selector, item.width, item.height, item.offsetX, item.offsetY));
                        }
                    }
                    Console.WriteLine("start call api");
                    var client = new RestClient("https://browser-service.herokuapp.com/dom/coordinates/" + url);
                    // client.Authenticator = new HttpBasicAuthenticator(username, password);
                    var request = new RestRequest();
                    request.AddParameter("url", url);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddJsonBody(requestData);
                    var response = client.Post(request);
                    var content = response.Content;

                    Console.WriteLine("done call api");
                    List<ResponseData> responseData = JsonConvert.DeserializeObject<List<ResponseData>>(content);

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
                        context.StatisticHeatmap.Add(statisticHeatmap);
                        try
                        {
                            context.SaveChanges();
                        }
                        catch (Exception)
                        {
                        }
                        
                    }


                }

                return max;

            }

        }


    }
}
