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
                List<TrackedHeatmapData> trackedList = context.TrackedHeatmapData.Where(s => s.TrackedHeatmapDataId > currentID).Where(s => EVENT_TYPE_LIST.Contains(s.EventType)).ToList();

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
                        //comment
                        //List<RequestData> requestData = new List<RequestData>();
                        foreach (var subItem in subTrackedList)
                        {
                            List<RequestData> requestData = new List<RequestData>();

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
                            var client = new RestClient("http://localhost:7777/dom/coordinates/" + url);
                            Console.WriteLine("\n===================================================================================\n" + "start call api: " + client.BaseUrl.ToString());

                            // client.Authenticator = new HttpBasicAuthenticator(username, password);
                            var request = new RestRequest();
                            request.AddParameter("url", url);
                            request.AddHeader("Content-Type", "application/json");
                            request.AddJsonBody(requestData);
                            var response = client.Post(request);
                            var content = response.Content;

                            Console.WriteLine(" done call api\n" + "\n===================================================================================\n" + "Response:\n");
                            Console.WriteLine(content.ToString() + "\n===================================================================================\n");



                            List<ResponseDataTemp> responseDataTemp = new List<ResponseDataTemp>();

                            try
                            {
                                responseDataTemp = JsonConvert.DeserializeObject<List<ResponseDataTemp>>(content);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("ERROR IN ID " + subItem.TrackedHeatmapDataId + ": " + ex.Message);
                            }

                            List<ResponseData> responseData = new List<ResponseData>();
                            foreach (var item in responseDataTemp)
                            {
                                if (item.x != null && item.y != null)
                                {
                                    responseData.Add(new ResponseData(item.trackedHeatmapDataID, Double.Parse(item.x), Double.Parse(item.y)));
                                }
                            }

                            List<int> uniqueTrackedHeatmapDataID = responseData.Select(s => s.trackedHeatmapDataID).Distinct().ToList();

                            foreach (var id in uniqueTrackedHeatmapDataID)
                            {
                                List<ResponseData> subData = responseData.Where(s => s.trackedHeatmapDataID == id).ToList();
                                StatisticHeatmap statisticHeatmapTest = context.StatisticHeatmap
                                    .Where(s => s.TrackedHeatmapDataId == id).FirstOrDefault();
                                if (statisticHeatmapTest == null)
                                {
                                    StatisticHeatmap statisticHeatmap = new StatisticHeatmap();
                                    statisticHeatmap.TrackedHeatmapDataId = id;
                                    List<StatisticData> statisticDatas = new List<StatisticData>();
                                    foreach (var item in subData)
                                    {
                                        statisticDatas.Add(new StatisticData(item.x, item.y));
                                    }
                                    statisticHeatmap.StatisticData = JsonConvert.SerializeObject(statisticDatas);
                                    context.StatisticHeatmap.Add(statisticHeatmap);

                                    context.SaveChanges();
                                }
                                else
                                {
                                    List<StatisticData> statisticDatas = new List<StatisticData>();
                                    foreach (var item in subData)
                                    {
                                        statisticDatas.Add(new StatisticData(item.x, item.y));
                                    }
                                    statisticHeatmapTest.StatisticData = JsonConvert.SerializeObject(statisticDatas);
                                    context.SaveChanges();
                                }

                            }
                        }

                        //var client = new RestClient("http://localhost:7777/dom/coordinates/" + url);
                        //Console.WriteLine("\n\n\n\n\n===================================================================================\n" + "start call api: " + client.BaseUrl.ToString());

                        //// client.Authenticator = new HttpBasicAuthenticator(username, password);
                        //var request = new RestRequest();
                        //request.AddParameter("url", url);
                        //request.AddHeader("Content-Type", "application/json");
                        //request.AddJsonBody(requestData);
                        //var response = client.Post(request);
                        //var content = response.Content;

                        //Console.WriteLine(" done call api\n" + "\n===================================================================================\n" + "Response:\n");
                        //Console.WriteLine(content.ToString());

                        //List<ResponseData> responseData = new List<ResponseData>();
                        //var settings = new JsonSerializerSettings
                        //{
                        //    NullValueHandling = NullValueHandling.Include,
                        //    MissingMemberHandling = MissingMemberHandling.Ignore
                        //};
                        //responseData = JsonConvert.DeserializeObject<List<ResponseData>>(content);

                        //List<int> uniqueTrackedHeatmapDataID = responseData.Select(s => s.trackedHeatmapDataID).Distinct().ToList();

                        //foreach (var id in uniqueTrackedHeatmapDataID)
                        //{
                        //    List<ResponseData> subData = responseData.Where(s => s.trackedHeatmapDataID == id).ToList();
                        //    StatisticHeatmap statisticHeatmap = new StatisticHeatmap();
                        //    statisticHeatmap.TrackedHeatmapDataId = id;
                        //    List<StatisticData> statisticDatas = new List<StatisticData>();
                        //    foreach (var item in subData)
                        //    {
                        //        statisticDatas.Add(new StatisticData(item.x, item.y));
                        //    }
                        //    statisticHeatmap.StatisticData = JsonConvert.SerializeObject(statisticDatas);
                        //    context.StatisticHeatmap.Add(statisticHeatmap);

                        //    context.SaveChanges();

                        //}
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\n===================================================================================\n"
                            + "ERROR from url " + url + " :" + ex.Message
                            + "\n===================================================================================\n");
                        continue;
                    }

                }

                return max;

            }

        }


    }
}
