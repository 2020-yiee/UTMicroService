using Newtonsoft.Json;
using RestSharp;
using StatisticAPIService.EFModels;
using StatisticAPIService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StatisticAPIService.Repository
{
    public class StatisticRepositoryImpl : IStatisticRepository
    {
        private readonly DBUTContext context = new DBUTContext();

        public object getMergeData(int webID, int eventType,string trackingUrl)
        {
            List<TrackedHeatmapData> datalist = context.TrackedHeatmapData
                .Where(s => s.WebId == webID)
                .Where(s => s.TrackingUrl == trackingUrl)
                .Where(s => s.EventType == eventType).ToList();

            List<data> data = new List<data>();
            foreach (var item in datalist)
            {
                data.AddRange(parseJson(item.Data));
            }
            return data;
        }

        public void updateStatisticData()
        {
            List<TrackedHeatmapData> trackedHeatmapDatas = checkDuplicateTrackedHeatmapDatas();
            foreach (TrackingHeatmapInfo infor in context.TrackingHeatmapInfo.ToList())
            {
                bool check = false;
                foreach (TrackedHeatmapData data in trackedHeatmapDatas)
                {
                    if (infor.TrackingUrl == data.TrackingUrl && infor.WebId == data.WebId) check = true;
                
                }
                if (check)
                {
                    int evenType = 1;
                    object data = getMergeData(infor.WebId, evenType, infor.TrackingUrl);
                    Console.WriteLine("call api to get statistic data");
                    var client = new RestClient("https://browser-service.herokuapp.com/coordinates");
                    // client.Authenticator = new HttpBasicAuthenticator(username, password);
                    var request = new RestRequest(infor.TrackingUrl);
                    request.AddParameter("url", infor.TrackingUrl);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddJsonBody(data);
                    var response = client.Post(request);
                    var content = response.Content;
                    StatisticHeatmap statisticHeatmap = new StatisticHeatmap();
                    statisticHeatmap.TrackingHeatmapInfoId = infor.TrackingHeatmapInfoId;
                    statisticHeatmap.StatisticData = content;
                    statisticHeatmap.EventType = evenType;
                    context.StatisticHeatmap.Add(statisticHeatmap);
                    context.SaveChanges();
                }
            }
        }

        public object updateStatisticData2()
        {
            List<TrackedHeatmapData> trackedHeatmapDatas = checkDuplicateTrackedHeatmapDatas();
            foreach (TrackingHeatmapInfo infor in context.TrackingHeatmapInfo.ToList())
            {
                bool check = false;
                foreach (TrackedHeatmapData data in trackedHeatmapDatas)
                {
                    if (infor.TrackingUrl == data.TrackingUrl && infor.WebId == data.WebId) check = true;

                }
                if (check)
                {
                    int evenType = 1;
                    object data = getMergeData(infor.WebId, evenType, infor.TrackingUrl);
                    Console.WriteLine("call api to get statistic data");
                    var client = new RestClient("https://browser-service.herokuapp.com/coordinates");
                    // client.Authenticator = new HttpBasicAuthenticator(username, password);
                    var request = new RestRequest(infor.TrackingUrl);
                    request.AddParameter("url", infor.TrackingUrl);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddJsonBody(data);
                    var response = client.Post(request);
                    var content = response.Content;
                    StatisticHeatmap statisticHeatmap = new StatisticHeatmap();
                    statisticHeatmap.TrackingHeatmapInfoId = infor.TrackingHeatmapInfoId;
                    statisticHeatmap.StatisticData = content;
                    statisticHeatmap.EventType = evenType;
                    context.StatisticHeatmap.Add(statisticHeatmap);
                    context.SaveChanges();
                    return statisticHeatmap;
                }
            }
            return null;
        }

        private List<TrackedHeatmapData> checkDuplicateTrackedHeatmapDatas()
        {
            List<TrackedHeatmapData> result = new List<TrackedHeatmapData>();
            List<int> ids = new List<int>();
            List<TrackedHeatmapData> datas = context.TrackedHeatmapData.ToList();
            foreach (TrackedHeatmapData data in datas)
            {
                if (!ids.Contains(data.WebId))
                {
                    result.Add(data);
                    ids.Add(data.WebId);
                }
            }
            return result;
        } 

        private List<data> parseJson(string data)
        {
            return JsonConvert.DeserializeObject<List<data>>(data);

        }
    }
}
